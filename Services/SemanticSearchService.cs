using Qdrant.Client;
using Qdrant.Client.Grpc;
using System.Net.Http.Json;
using System.Text.Json;

namespace PortfoyTakipAPI.Services
{
    public interface ISemanticSearchService
    {
        Task VeritabaniHazirlaAsync();
        Task<bool> MetniOgretAsync(ulong id, string metin);
        // YENİ: Arama metodu arayüze eklendi
        Task<List<string>> BenzerMetinleriBulAsync(string soru, int limit = 2);
    }

    public class SemanticSearchService : ISemanticSearchService
    {
        private readonly QdrantClient _qdrantClient;
        private readonly HttpClient _httpClient;
        private readonly string _collectionName = "sirket_kurallari";

        public SemanticSearchService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _qdrantClient = new QdrantClient("localhost", 6334);
        }

        public async Task VeritabaniHazirlaAsync()
        {
            var exists = await _qdrantClient.CollectionExistsAsync(_collectionName);
            if (!exists)
            {
                await _qdrantClient.CreateCollectionAsync(
                    collectionName: _collectionName,
                    vectorsConfig: new VectorParams { Size = 768, Distance = Distance.Cosine }
                );
            }
        }

        public async Task<bool> MetniOgretAsync(ulong id, string metin)
        {
            var requestBody = new { model = "nomic-embed-text", prompt = metin };
            var response = await _httpClient.PostAsJsonAsync("http://localhost:11434/api/embeddings", requestBody);
            var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

            var vectorArray = jsonResponse.GetProperty("embedding").EnumerateArray();
            var floatList = new List<float>();
            foreach (var item in vectorArray) floatList.Add(item.GetSingle());

            var point = new PointStruct
            {
                Id = id,
                Vectors = floatList.ToArray(),
                Payload = { ["icerik"] = metin }
            };

            await _qdrantClient.UpsertAsync(_collectionName, new[] { point });
            return true;
        }

        // YENİ: RAG MİMARİSİNİN ARAMA KISMI
        public async Task<List<string>> BenzerMetinleriBulAsync(string soru, int limit = 2)
        {
            try
            {
                // 1. Kullanıcının sorusunu vektöre çevir
                var requestBody = new { model = "nomic-embed-text", prompt = soru };
                var response = await _httpClient.PostAsJsonAsync("http://localhost:11434/api/embeddings", requestBody);
                var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

                var vectorArray = jsonResponse.GetProperty("embedding").EnumerateArray();
                var floatList = new List<float>();
                foreach (var item in vectorArray) floatList.Add(item.GetSingle());

                // 2. Qdrant'ta benzer vektörleri ara
                var searchResult = await _qdrantClient.SearchAsync(
                    collectionName: _collectionName,
                    vector: floatList.ToArray(),
                    limit: (ulong)limit
                );

                // 3. Bulunan kayıtların içindeki orijinal metinleri listeye doldur
                var bulunanMetinler = new List<string>();
                foreach (var result in searchResult)
                {
                    if (result.Payload.TryGetValue("icerik", out var icerikValue))
                    {
                        bulunanMetinler.Add(icerikValue.StringValue);
                    }
                }

                return bulunanMetinler;
            }
            catch
            {
                return new List<string>(); // Hata olursa boş liste dön
            }
        }
    }
}