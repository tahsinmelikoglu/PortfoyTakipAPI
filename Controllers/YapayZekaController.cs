using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using PortfoyTakipAPI.Models;

namespace PortfoyTakipAPI.Controllers
{
    [Authorize] // Kilitli kapı kuralını burası için de unutmuyoruz!
    [Route("api/[controller]")]
    [ApiController]
    public class YapayZekaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;

        public YapayZekaController(AppDbContext context)
        {
            _context = context;
            _httpClient = new HttpClient(); // Ollama ile konuşacak olan telsizimiz
        }

        [HttpGet("analiz")]
        public async Task<IActionResult> PortfoyAnaliziAl()
        {
            // 1. Kilerdeki (MSSQL) tüm varlıkları çek
            var varliklar = await _context.Varliklar.ToListAsync();

            if (varliklar.Count == 0)
                return BadRequest("Analiz edilecek herhangi bir veri yok.");

            // 2. Yapay zekaya verilecek emri (Prompt) hazırla
            var promptBuilder = new StringBuilder();
            promptBuilder.AppendLine("Sen profesyonel bir finans ve portföy danışmanısın. Aşağıdaki portföy verilerine ve tasarruf/bakiye durumuna bakarak bana kısa, net ve stratejik bir Türkçe analiz yap:");

            // Kilerdeki her bir altını/hisse senedini tek tek listeye ekle
            foreach (var v in varliklar)
            {
                promptBuilder.AppendLine($"- Tür: {v.VarlikTuru}, Sembol: {v.Sembol}, Miktar: {v.Miktar}, Bakiye Durumu: {v.Bakiye}");
            }

            // 3. Ollama'ya gönderilecek veri paketini (JSON) hazırla
            var ollamaIstek = new
            {
                model = "llama3", // İndirdiğimiz beyin
                prompt = promptBuilder.ToString(),
                stream = false // Kelime kelime değil, cevabı toptan ver
            };

            var content = new StringContent(JsonSerializer.Serialize(ollamaIstek), Encoding.UTF8, "application/json");

            try
            {
                // 4. Ollama motoruna (11434 portu) isteği ateşle
                var response = await _httpClient.PostAsync("http://localhost:11434/api/generate", content);
                var responseString = await response.Content.ReadAsStringAsync();

                // 5. Gelen cevabı ayrıştır ve müşteriye ilet
                using var jsonDoc = JsonDocument.Parse(responseString);
                var aiCevabi = jsonDoc.RootElement.GetProperty("response").GetString();

                return Ok(new { portfoyAnalizi = aiCevabi });
            }
            catch (Exception)
            {
                return StatusCode(500, "Yapay zeka motoruna ulaşılamadı. Arka planda Ollama'nın çalıştığından emin olun.");
            }
        }
    }
}