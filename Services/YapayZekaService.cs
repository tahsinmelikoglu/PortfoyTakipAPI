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
        Task<string> PortfoyAnaliziYapAsync(string prompt);
    }

    public class YapayZekaService : IYapayZekaService
    {
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _cache;
        private readonly IVarlikRepository _repository; // Kilere inip verileri alabilmemiz için gerekli

        public YapayZekaService(HttpClient httpClient, IDistributedCache cache, IVarlikRepository repository)
        {
            _httpClient = httpClient;
            _cache = cache;
            _repository = repository;
        }

        public async Task<string> PortfoyAnaliziYapAsync(string prompt)
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

            // 3. OLLAMA İÇİN ÇELİK GİBİ SERT BİR SİSTEM PROMPTU (KURALLAR BÜTÜNÜ) YAZIYORUZ
            string systemPrompt = $@"
Sen profesyonel bir Türk portföy yönetim asistanısın. 
Görevin SADECE TÜRKÇE olarak finansal tavsiye vermektir. İngilizce veya başka bir dil kullanman KESİNLİKLE YASAKTIR.
Hayal ürünü hisse senetleri (örneğin KARÇİL vb.) uydurmayacaksın.

Aşağıda kullanıcının veritabanından çekilen GERÇEK portföy verileri bulunmaktadır:
[PORTFÖY BAŞLANGICI]
{portfoyOzeti.ToString()}
[PORTFÖY SONU]

Sadece bu yukarıdaki gerçek verilere dayanarak, kullanıcının aşağıdaki sorusuna mantıklı, profesyonel ve tamamen Türkçe bir cevap ver.

Kullanıcı Sorusu: {prompt}";

            // 4. İSTEĞİ HAZIRLA VE OLLAMA'YA GÖNDER
            var requestBody = new
            {
                model = "llama3",
                prompt = systemPrompt,
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