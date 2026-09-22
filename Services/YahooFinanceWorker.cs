using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PortfoyTakipAPI.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PortfoyTakipAPI.Services
{
    public class YahooFinanceWorker : BackgroundService
    {
        private readonly ILogger<YahooFinanceWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly HttpClient _httpClient;

        // Borsa saatleri dışında servisi uyutmak için kullanacağımız aralık
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(15);

        public YahooFinanceWorker(ILogger<YahooFinanceWorker> logger, IServiceScopeFactory scopeFactory, HttpClient httpClient)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _httpClient = httpClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("YahooFinanceWorker başlatıldı.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await BorsaVerileriniGuncelle(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "YahooFinanceWorker çalışırken bir hata oluştu.");
                }

                _logger.LogInformation($"YahooFinanceWorker uykuya geçti. {_checkInterval.TotalMinutes} dakika sonra tekrar çalışacak.");
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task BorsaVerileriniGuncelle(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Sadece "İşlem Gören" statüsündeki şirketleri getir
            var islemGorenSirketler = await dbContext.HalkaArzlar
                .Where(h => h.Statu == "İşlem Gören")
                .ToListAsync(stoppingToken);

            if (!islemGorenSirketler.Any())
            {
                _logger.LogInformation("İşlem gören şirket bulunamadı. Veri çekme işlemi atlandı.");
                return;
            }

            _logger.LogInformation($"{islemGorenSirketler.Count} adet işlem gören şirket için veriler güncelleniyor...");

            foreach (var sirket in islemGorenSirketler)
            {
                string yahooSymbol = $"{sirket.BorsaKodu}.IS";

                try
                {
                    string apiUrl = $"https://query1.finance.yahoo.com/v8/finance/chart/{yahooSymbol}?range=5d&interval=1h";

                    _httpClient.DefaultRequestHeaders.Clear();
                    _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

                    var response = await _httpClient.GetAsync(apiUrl, stoppingToken);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync(stoppingToken);
                        using var doc = JsonDocument.Parse(jsonString);
                        var root = doc.RootElement;
                        var result = root.GetProperty("chart").GetProperty("result")[0];

                        var meta = result.GetProperty("meta");
                        var indicators = result.GetProperty("indicators").GetProperty("quote")[0];

                        // 1. Kapanış fiyatlarını (close) al
                        var closePricesElement = indicators.GetProperty("close");
                        var fiyatListesi = new List<decimal>();

                        foreach (var price in closePricesElement.EnumerateArray())
                        {
                            if (price.ValueKind == JsonValueKind.Number)
                            {
                                fiyatListesi.Add((decimal)price.GetDouble());
                            }
                        }

                        // 2. Zaman damgalarını (timestamp) al ve biçimlendir
                        var zamanListesi = new List<string>();
                        if (result.TryGetProperty("timestamp", out var timestampsElement))
                        {
                            foreach (var ts in timestampsElement.EnumerateArray())
                            {
                                if (ts.ValueKind == JsonValueKind.Number)
                                {
                                    long unixTime = ts.GetInt64();
                                    DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTime).LocalDateTime;
                                    zamanListesi.Add(dateTime.ToString("dd.MM HH:mm"));
                                }
                            }
                        }

                        if (fiyatListesi.Any())
                        {
                            decimal guncelFiyat = fiyatListesi.Last();

                            decimal dunkuKapanis = guncelFiyat;
                            if (fiyatListesi.Count > 1)
                            {
                                dunkuKapanis = fiyatListesi[fiyatListesi.Count - 2];
                            }
                            else if (meta.TryGetProperty("regularMarketPreviousClose", out var prevClose))
                            {
                                dunkuKapanis = (decimal)prevClose.GetDouble();
                            }

                            decimal gunlukDegisim = 0;
                            if (dunkuKapanis > 0)
                            {
                                gunlukDegisim = ((guncelFiyat - dunkuKapanis) / dunkuKapanis) * 100;
                            }

                            // Veritabanı modelini güncelle
                            sirket.GuncelFiyat = Math.Round(guncelFiyat, 2);
                            sirket.GunlukDegisimYuzdesi = Math.Round(gunlukDegisim, 2);
                            sirket.FiyatGecmisiJson = JsonSerializer.Serialize(fiyatListesi.Select(p => Math.Round(p, 2)).ToList());
                            sirket.ZamanGecmisiJson = JsonSerializer.Serialize(zamanListesi); // Zaman damgaları eklendi
                            sirket.BorsaVerisiSonGuncelleme = DateTime.Now;

                            _logger.LogInformation($"Başarılı: {sirket.BorsaKodu} - Fiyat: {sirket.GuncelFiyat} TL, Değişim: %{sirket.GunlukDegisimYuzdesi}");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Hata: {sirket.BorsaKodu} için Yahoo Finance API isteği başarısız oldu. StatusCode: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Hata: {sirket.BorsaKodu} için Yahoo Finance'den veri çekilirken hata oluştu: {ex.Message}");
                }

                await Task.Delay(1000, stoppingToken);
            }

            await dbContext.SaveChangesAsync(stoppingToken);
            _logger.LogInformation("Tüm canlı veriler veritabanına başarıyla kaydedildi.");
        }
    }
}