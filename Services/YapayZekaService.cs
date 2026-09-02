using Microsoft.Extensions.Caching.Distributed;
using PortfoyTakipAPI.Repositories;
using PortfoyTakipAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace PortfoyTakipAPI.Services
{
    public interface IYapayZekaService
    {
        Task<string> PortfoyAnaliziYapAsync(string sessionId, string prompt, string riskProfili);
        Task<decimal> CanliFiyatGetirAsync(string sembol);
    }

    public class YapayZekaService : IYapayZekaService
    {
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _cache;
        private readonly IVarlikRepository _repository;
        private readonly ISemanticSearchService _semanticSearchService;
        private readonly AppDbContext _context;

        public YapayZekaService(HttpClient httpClient, IDistributedCache cache, IVarlikRepository repository, ISemanticSearchService semanticSearchService, AppDbContext context)
        {
            _httpClient = httpClient;
            _cache = cache;
            _repository = repository;
            _semanticSearchService = semanticSearchService;
            _context = context;
        }

        // =========================================================
        // YENİ: YAHOO FINANCE ÜZERİNDEN CANLI FİYAT ÇEKME METODU
        // =========================================================
        public async Task<decimal> CanliFiyatGetirAsync(string sembol)
        {
            try
            {
                // BİST hisseleri için sonuna .IS ekliyoruz (Örn: THYAO.IS)
                string borsaSembolu = sembol.ToUpper().EndsWith(".IS") ? sembol.ToUpper() : $"{sembol.ToUpper()}.IS";
                string url = $"https://query1.finance.yahoo.com/v8/finance/chart/{borsaSembolu}";

                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var result = jsonResponse.GetProperty("chart").GetProperty("result")[0];
                    var fiyat = result.GetProperty("meta").GetProperty("regularMarketPrice").GetDecimal();
                    return fiyat;
                }
            }
            catch
            {
                // API çökerse veya hisse bulunamazsa program patlamasın
            }

            return -1m;
        }

        public async Task<string> PortfoyAnaliziYapAsync(string sessionId, string prompt, string riskProfili)
        {
            var url = "http://localhost:11434/api/chat";

            // 1. KİLERDEN SADECE BU KULLANICININ GERÇEK VARLIKLARINI ÇEK
            // sessionId değişkeni artık JWT'den gelen gerçek Kullanıcı ID'sini taşıyor
            var mevcutVarliklar = _repository.GetAll()
                                             .Where(v => v.KullaniciId.ToString() == sessionId)
                                             .ToList();
            var portfoyOzeti = new StringBuilder();

            if (mevcutVarliklar.Any())
            {
                foreach (var v in mevcutVarliklar)
                {
                    // YENİ: Canlı fiyatı API'den bekliyoruz
                    decimal guncelFiyat = await CanliFiyatGetirAsync(v.Sembol);

                    if (guncelFiyat == -1m)
                    {
                        portfoyOzeti.AppendLine($"- Sembol: {v.Sembol}, Tür: {v.VarlikTuru}, Miktar: {v.Miktar}, FİYAT ÇEKİLEMEDİ.");
                        continue;
                    }

                    decimal toplamMaliyet = (decimal)v.AlisFiyati * (decimal)v.Miktar;
                    decimal guncelDeger = guncelFiyat * (decimal)v.Miktar;
                    decimal karZarar = guncelDeger - toplamMaliyet;

                    portfoyOzeti.AppendLine($"- Sembol: {v.Sembol}, Tür: {v.VarlikTuru}, Miktar: {v.Miktar}, Alış: {v.AlisFiyati} TL, Güncel Fiyat: {guncelFiyat:F2} TL, Net Kâr/Zarar: {karZarar:F2} TL");
                }
            }
            else
            {
                portfoyOzeti.AppendLine("Kullanıcının portföyünde şu an hiçbir varlık bulunmamaktadır.");
            }

            // 2. QDRANT'TAN KURALLARI ÇEK
            var benzerKurallar = await _semanticSearchService.BenzerMetinleriBulAsync(prompt);
            var kurallarMetni = new StringBuilder();
            if (benzerKurallar.Any())
            {
                foreach (var kural in benzerKurallar)
                    kurallarMetni.AppendLine($"- {kural}");
            }
            else
            {
                kurallarMetni.AppendLine("Bu soruyla eşleşen özel bir şirket kuralı bulunamadı.");
            }

            // 3. KARAKTER SEÇİMİ
            string karakterTalimati = riskProfili.ToLower() switch
            {
                "garantici" => "Sen çok temkinli, riskten nefret eden geleneksel bir bankacısın.",
                "agresif" => "Sen Wall Street'te çalışan çok cesur bir daytrader'sın.",
                _ => "Sen dengeli bir portföy yönetimi sunan modern bir finans uzmanısın."
            };

            // 4. SİSTEM PROMPTU (Matematik yasağı eklendi)
            string systemPrompt = $@"
ROLÜN: Kıdemli ve net bir portföy analistisin. Kesinlikle TÜRKÇE cevaplar ver. Kesinlikle lafı uzatma, süslü ve uyduruk kurumsal cümleler kurma. 

KURALLAR:
1. SADECE aşağıdaki GERÇEK portföy verilerine ve ŞİRKET KURALLARINA dayanarak cevap ver. Veride olmayan hiçbir şeyi asla uydurma.
2. PORTFÖY VERİLERİ KISMINDA KÂR VE ZARAR HESAPLAMALARI ZATEN YAPILMIŞTIR. ASLA kendi kendine matematiksel hesaplama yapma, formül kurma veya fiyatları çarpma. Sadece sana verilen 'Net Kâr/Zarar' rakamlarını oku ve metin olarak raporla.
3. Asla spekülasyon yapma, kesin ve kısa yanıtlar ver.
4. Yanıtının en sonunda mutlaka 'Yatırım Tavsiyesi Değildir' ibaresine yer ver.

[GERÇEK PORTFÖY VERİLERİ]{portfoyOzeti.ToString()}

[ŞİRKET KURALLARI]{kurallarMetni.ToString()}
";

            // =========================================================
            // 5. GÜNCELLENEN: HAFIZA YÖNETİMİ (Docker Şişmesini Önler)
            // =========================================================
            var history = await _context.ChatHistories
                .Where(h => h.SessionId == sessionId)
                .OrderByDescending(h => h.CreatedAt) // En yenileri al
                .Take(8) // Sadece son 4 mesajı (2 soru, 2 cevap) tut
                .ToListAsync();

            history.Reverse(); // Modele kronolojik sırayla vermek için tersine çevir

            _context.ChatHistories.Add(new ChatHistory { SessionId = sessionId, Role = "user", Message = prompt });
            await _context.SaveChangesAsync();

            var messages = new List<object>
            {
                new { role = "system", content = systemPrompt }
            };

            foreach (var h in history)
            {
                messages.Add(new { role = h.Role, content = h.Message });
            }

            messages.Add(new { role = "user", content = prompt });

            // 6. OLLAMA'YA GÖNDER
            var requestBody = new
            {
                model = "llama3",
                messages = messages,
                stream = false,
                options = new
                {
                    temperature = 0.0
                }
            };

            var response = await _httpClient.PostAsJsonAsync(url, requestBody);
            if (!response.IsSuccessStatusCode)
            {
                return "Yapay zeka servisine ulaşılamadı.";
            }

            var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
            var answer = jsonResponse.GetProperty("message").GetProperty("content").GetString();

            // 7. ASİSTANIN YANITINI KAYDET
            _context.ChatHistories.Add(new ChatHistory { SessionId = sessionId, Role = "assistant", Message = answer });
            await _context.SaveChangesAsync();

            return answer ?? "Yapay zeka boş bir yanıt döndürdü.";
        }
    }
}