using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PortfoyTakipAPI.Services
{
    public interface IBistService
    {
        Task<decimal> GetHisseFiyatiAsync(string borsaKodu);
    }

    public class BistService : IBistService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BistService> _logger;

        public BistService(HttpClient httpClient, ILogger<BistService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            // Yahoo Finance bot olduğumuzu sanıp engellemesin diye sahte bir tarayıcı kimliği veriyoruz
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        }

        public async Task<decimal> GetHisseFiyatiAsync(string borsaKodu)
        {
            try
            {
                // BİST hisseleri için Yahoo Finance formatı: THYAO.IS
                string sembol = $"{borsaKodu.ToUpper().Trim()}.IS";
                string url = $"https://query1.finance.yahoo.com/v8/finance/chart/{sembol}";

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("{BorsaKodu} için fiyat çekilemedi. HTTP Status: {Status}", borsaKodu, response.StatusCode);
                    return 0;
                }

                var jsonString = await response.Content.ReadAsStringAsync();

                // JSON parsing işlemi
                using var document = JsonDocument.Parse(jsonString);
                var root = document.RootElement;

                var result = root.GetProperty("chart").GetProperty("result")[0];
                var meta = result.GetProperty("meta");
                var currentPrice = meta.GetProperty("regularMarketPrice").GetDecimal();

                return currentPrice;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{BorsaKodu} fiyatı çekilirken bir hata oluştu.", borsaKodu);
                return 0; // Hata durumunda 0 dönüyoruz ki sistem çökmesin
            }
        }
    }
}