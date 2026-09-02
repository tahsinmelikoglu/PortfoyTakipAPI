using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfoyTakipAPI.Services;
using System;
using System.Security.Claims; // YENİ: JWT içindeki kimliği okumak için
using System.Threading.Tasks;

namespace PortfoyTakipAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Güvenlik kalkanımız burada da aktif (Token zorunlu)
    public class YapayZekaController : ControllerBase
    {
        private readonly IYapayZekaService _yapayZekaService;
        private readonly ISemanticSearchService _semanticSearchService;

        public YapayZekaController(IYapayZekaService yapayZekaService, ISemanticSearchService semanticSearchService)
        {
            _yapayZekaService = yapayZekaService;
            _semanticSearchService = semanticSearchService;
        }

        [HttpPost("analiz")]
        public async Task<IActionResult> PortfoyYorumla([FromBody] PromptRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Soru))
            {
                return BadRequest(new { message = "Lütfen yapay zekaya sormak istediğiniz soruyu belirtin." });
            }

            // --- YENİ EKLENEN GÜVENLİK DUVARI ---
            // İstek atan kullanıcının Token'ı içinden kendi ID'sini yakalıyoruz
            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(kullaniciId))
            {
                return Unauthorized(new { message = "Geçersiz kullanıcı oturumu." });
            }

            // "oturum-123" yerine artık GERÇEK kullanıcı kimliğini gönderiyoruz
            string aiYaniti = await _yapayZekaService.PortfoyAnaliziYapAsync(kullaniciId, request.Soru, request.RiskProfili);

            return Ok(new
            {
                soru = request.Soru,
                yapayZekaYorumu = aiYaniti
            });
        }

        [HttpPost("ogret")]
        public async Task<IActionResult> BilgiOgret([FromBody] OgretRequest request)
        {
            await _semanticSearchService.VeritabaniHazirlaAsync();
            bool sonuc = await _semanticSearchService.MetniOgretAsync(request.Id, request.Metin);

            if (sonuc)
            {
                return Ok(new { mesaj = "Bilgi başarıyla yapay zekanın hafızasına (Qdrant) kazındı!" });
            }

            return BadRequest("Bilgi öğrenilirken bir hata oluştu.");
        }

        [HttpGet("hata-test")]
        [AllowAnonymous]
        public IActionResult HataTest()
        {
            throw new Exception("Kritik Sistem Hatası: Veritabanı bağlantısı koptu! SQL Server 30 saniyedir yanıt vermiyor.");
        }
    }

    public class PromptRequest
    {
        public string Soru { get; set; }
        public string RiskProfili { get; set; } = "Dengeli";
    }

    public class OgretRequest
    {
        public ulong Id { get; set; }
        public string Metin { get; set; }
    }
}