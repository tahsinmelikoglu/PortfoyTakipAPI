using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace PortfoyTakipAPI.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        private static readonly HttpClient _httpClient = new HttpClient();

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Beklenmeyen Kritik Bir Hata Oluştu: {Message}", ex.Message);
                await LlmIleHatayiOzetleAsync(ex);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task LlmIleHatayiOzetleAsync(Exception ex)
        {
            try
            {
                var logDir = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
                if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);

                var fileName = $"Exception_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                var filePath = Path.Combine(logDir, fileName);

                var hataMetni = $"Hata Mesajı: {ex.Message}\nStack Trace: {ex.StackTrace}";
                await File.WriteAllTextAsync(filePath, hataMetni);

                // PROMPT GÜNCELLENDİ: Sıfır inisiyatif, kesin şablon, düzgün Türkçe emri!
                var prompt = $@"Aşağıdaki sistem hatasını özetle.
SADECE TÜRKÇE YANIT VER. İmla kurallarına kesinlikle uy, hatalı harf kullanma.
Yanıtını SADECE aşağıdaki şablona birebir uyarak ver, ekstra tek bir kelime bile yazma:

Özet: [Buraya 2 cümlelik düzgün bir Türkçe özet yaz]
Seviye: KRİTİK

Hata Logu: 
{hataMetni}";

                var requestBody = new
                {
                    model = "llama3",
                    prompt = prompt,
                    stream = false
                };

                var response = await _httpClient.PostAsJsonAsync("http://localhost:11434/api/generate", requestBody);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var llmCevabi = jsonResponse.GetProperty("response").GetString() ?? "";

                    Console.WriteLine("\n================ LLM HATA ANALİZİ ================");

                    // KONTROL GÜNCELLENDİ: Büyük harfe çevirip direkt kelimeyi arıyoruz, köşeli parantez tuzağına düşmüyoruz
                    var buyukHarfliCevap = llmCevabi.ToUpper(new System.Globalization.CultureInfo("tr-TR"));

                    if (buyukHarfliCevap.Contains("KRİTİK"))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else if (buyukHarfliCevap.Contains("ORTA"))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }

                    Console.WriteLine($"Kaydedilen Dosya: {fileName}");
                    Console.WriteLine(llmCevabi);
                    Console.ResetColor();
                    Console.WriteLine("==================================================\n");
                }
            }
            catch (Exception llmEx)
            {
                _logger.LogWarning("LLM hata özeti çıkarırken bir sorun oluştu: {Message}", llmEx.Message);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Sunucu tarafında beklenmeyen bir hata oluştu. Teknik ekibimiz logları inceliyor.",
                Detailed = exception.Message
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}