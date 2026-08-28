using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfoyTakipAPI.Services;
using System;
using System.Threading.Tasks;

namespace PortfoyTakipAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Güvenlik kalkanımız burada da aktif (Token zorunlu)
    public class YapayZekaController : ControllerBase
    {
        private readonly IYapayZekaService _yapayZekaService;
        private readonly ISemanticSearchService _semanticSearchService; // YENİ: Vektör DB Servisimiz

        // YAPICI METOT GÜNCELLENDİ
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

            string aiYaniti = await _yapayZekaService.PortfoyAnaliziYapAsync(request.Soru, request.RiskProfili);

            return Ok(new
            {
                soru = request.Soru,
                yapayZekaYorumu = aiYaniti
            });
        }

        // YENİ: YAPAY ZEKAYA KURUMSAL BİLGİ/KURAL ÖĞRETME UCU
        [HttpPost("ogret")]
        public async Task<IActionResult> BilgiOgret([FromBody] OgretRequest request)
        {
            // 1. Qdrant'ta kolleksiyon (tablo) yoksa oluştur
            await _semanticSearchService.VeritabaniHazirlaAsync();

            // 2. Metni 768 boyutlu vektöre çevir ve Qdrant'a kaydet
            bool sonuc = await _semanticSearchService.MetniOgretAsync(request.Id, request.Metin);

            if (sonuc)
            {
                return Ok(new { mesaj = "Bilgi başarıyla yapay zekanın hafızasına (Qdrant) kazındı!" });
            }

            return BadRequest("Bilgi öğrenilirken bir hata oluştu.");
        }

        // TEST METODU ŞİMDİ DOĞRU YERDE (CLASS'IN İÇİNDE)
        [HttpGet("hata-test")]
        [AllowAnonymous] // Sadece test edeceğimiz için Token sormasın
        public IActionResult HataTest()
        {
            // Bilerek kritik bir veritabanı hatası simüle ediyoruz
            throw new Exception("Kritik Sistem Hatası: Veritabanı bağlantısı koptu! SQL Server 30 saniyedir yanıt vermiyor.");
        }
    }

    public class PromptRequest
    {
        public string Soru { get; set; }

        // Kullanıcının yatırım tarzını belirleyen alan
        // (Örn: "Garantici", "Dengeli", "Agresif")
        public string RiskProfili { get; set; } = "Dengeli"; // Varsayılan olarak Dengeli olsun
    }

    // YENİ: ÖĞRETME İSTEĞİ İÇİN DTO
    public class OgretRequest
    {
        public ulong Id { get; set; }
        public string Metin { get; set; }
    }
}