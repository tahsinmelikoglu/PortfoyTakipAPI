using Microsoft.Extensions.Caching.Distributed;
using PortfoyTakipAPI.Repositories;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

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
        private readonly ISemanticSearchService _semanticSearchService; // YENİ: Vektör Arama Servisi

        // YAPICI METOT GÜNCELLENDİ (Bağımlılık eklendi)
        public YapayZekaService(HttpClient httpClient, IDistributedCache cache, IVarlikRepository repository, ISemanticSearchService semanticSearchService)
        {
            _httpClient = httpClient;
            _cache = cache;
            _repository = repository;
            _semanticSearchService = semanticSearchService;
        }

        public async Task<string> PortfoyAnaliziYapAsync(string prompt, string riskProfili)
        {
            var url = "http://localhost:11434/api/generate";

            // 1. KİLERDEN (VERİTABANINDAN) GERÇEK VARLIKLARINI ÇEK
            var mevcutVarliklar = _repository.GetAll().ToList();

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

            // ====================================================================
            // 2. YENİ: RAG MİMARİSİ (RETRIEVAL) - QDRANT'TAN KURALLARI ÇEK
            // ====================================================================
            var benzerKurallar = await _semanticSearchService.BenzerMetinleriBulAsync(prompt);
            var kurallarMetni = new StringBuilder();

            if (benzerKurallar.Any())
            {
                foreach (var kural in benzerKurallar)
                {
                    kurallarMetni.AppendLine($"- {kural}");
                }
            }
            else
            {
                kurallarMetni.AppendLine("Bu soruyla eşleşen özel bir şirket kuralı bulunamadı.");
            }

            // 3. KULLANICI PROFİLİNE GÖRE KARAKTER SEÇİMİ
            string karakterTalimati = "";
            switch (riskProfili.ToLower())
            {
                case "garantici":
                    karakterTalimati = "Sen çok temkinli, riskten nefret eden ve anaparayı korumayı her şeyden üstün tutan geleneksel bir bankacısın.";
                    break;
                case "agresif":
                    karakterTalimati = "Sen Wall Street'te çalışan, yüksek risk ve yüksek getiri aşığı, çok cesur bir daytrader'sın.";
                    break;
                default:
                    karakterTalimati = "Sen mantıklı, riskleri dağıtmayı seven ve dengeli bir portföy yönetimi sunan modern bir finans uzmanısın.";
                    break;
            }

            // ====================================================================
            // 4. SİSTEM PROMPTUNU ZENGİNLEŞTİR (AUGMENTED GENERATION)
            // ====================================================================
            string systemPrompt = $@"
Sen profesyonel bir Türk portföy yönetim asistanısın. 
{karakterTalimati}
Görevin SADECE TÜRKÇE olarak finansal tavsiye vermektir.

Aşağıda kullanıcının veritabanından çekilen GERÇEK portföy verileri bulunmaktadır:
[PORTFÖY BAŞLANGICI]
{portfoyOzeti.ToString()}
[PORTFÖY SONU]

Aşağıda ise şirketimizin uyman gereken kati kuralları bulunmaktadır:
[ŞİRKET KURALLARI BAŞLANGICI]
{kurallarMetni.ToString()}
[ŞİRKET KURALLARI SONU]

Sadece bu yukarıdaki gerçek verilere ve şirket kurallarına dayanarak kullanıcının sorusuna cevap ver.";

            var requestBody = new
            {
                model = "llama3",
                system = systemPrompt,
                prompt = prompt,
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