using Microsoft.Extensions.Caching.Distributed;
using PortfoyTakipAPI.Repositories;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PortfoyTakipAPI.Services
{
    public interface IYapayZekaService
    {
        Task<string> PortfoyAnaliziYapAsync(string prompt, string riskProfili);
    }

    public class YapayZekaService : IYapayZekaService
    {
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _cache;
        private readonly IVarlikRepository _repository; 

        public YapayZekaService(HttpClient httpClient, IDistributedCache cache, IVarlikRepository repository)
        {
            _httpClient = httpClient;
            _cache = cache;
            _repository = repository;
        }

        public async Task<string> PortfoyAnaliziYapAsync(string prompt, string riskProfili)
        {
            var url = "http://localhost:11434/api/generate";

            // 1. KİLERDEN (VERİTABANINDAN) GERÇEK VARLIKLARINI ÇEK
            var mevcutVarliklar = _repository.GetAll().ToList();

            // 2. VERİLERİ YAPAY ZEKANIN OKUYABİLECEĞİ METNE ÇEVİR
            var portfoyOzeti = new StringBuilder();
            if (mevcutVarliklar.Any())
            {
                foreach (var v in mevcutVarliklar)
                {
                    portfoyOzeti.AppendLine($"- Sembol: {v.Sembol}, Tür: {v.VarlikTuru}, Miktar: {v.Miktar}");
                }
            }
            else
            {
                portfoyOzeti.AppendLine("Kullanıcının portföyünde şu an hiçbir varlık bulunmamaktadır.");
            }

            // 3. KULLANICI PROFİLİNE GÖRE KARAKTER SEÇİMİ (DİNAMİK PROMPTING)
            string karakterTalimati = "";
            switch (riskProfili.ToLower())
            {
                case "garantici":
                    karakterTalimati = "Sen çok temkinli, riskten nefret eden ve anaparayı korumayı her şeyden üstün tutan geleneksel bir bankacısın. Kullanıcıya her zaman en güvenli ve risksiz yolu tavsiye et.";
                    break;
                case "agresif":
                    karakterTalimati = "Sen Wall Street'te çalışan, yüksek risk ve yüksek getiri aşığı, çok cesur bir daytrader'sın. Kullanıcıya cesur hamleler, kripto paralar ve agresif büyüme stratejileri tavsiye et.";
                    break;
                default:
                    karakterTalimati = "Sen mantıklı, riskleri dağıtmayı seven ve dengeli bir portföy yönetimi sunan modern bir finans uzmanısın.";
                    break;
            }

            // 4. SİSTEM PROMPTUNU (KİŞİLİK VE KURALLARI) AYRI HAZIRLA
            string systemPrompt = $@"
Sen profesyonel bir Türk portföy yönetim asistanısın. 
{karakterTalimati}
Görevin SADECE TÜRKÇE olarak finansal tavsiye vermektir. İngilizce veya başka bir dil kullanman KESİNLİKLE YASAKTIR.
Hayal ürünü hisse senetleri uydurmayacaksın.

Aşağıda kullanıcının veritabanından çekilen GERÇEK portföy verileri bulunmaktadır:
[PORTFÖY BAŞLANGICI]
{portfoyOzeti.ToString()}
[PORTFÖY SONU]

Sadece bu yukarıdaki gerçek verilere dayanarak kullanıcının sorusuna mantıklı, profesyonel ve tamamen Türkçe bir cevap ver.";

            // 5. İSTEĞİ OLLAMA API'SİNE UYGUN ŞEKİLDE BÖLEREK GÖNDER
            var requestBody = new
            {
                model = "llama3",
                system = systemPrompt, // YAPAY ZEKANIN KİŞİLİĞİ VE KURALLARI (ASLA UNUTMAZ)
                prompt = prompt,       // KULLANICININ SADECE SORUSU
                stream = false
            };

            var response = await _httpClient.PostAsJsonAsync(url, requestBody);
            if (!response.IsSuccessStatusCode)
            {
                return "Yapay zeka servisine ulaşılamadı. Ollama'nın çalıştığından emin olun.";
            }

            var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

            if (jsonResponse.TryGetProperty("response", out var answer))
            {
                return answer.GetString() ?? "Yapay zeka boş bir yanıt döndürdü.";
            }

            return "Yanıt işlenirken bir hata oluştu.";
        }
    }
}