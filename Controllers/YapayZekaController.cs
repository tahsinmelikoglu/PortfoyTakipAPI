using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfoyTakipAPI.Services;
using System.Threading.Tasks;

namespace PortfoyTakipAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Güvenlik kalkanımız burada da aktif (Token zorunlu)
    public class YapayZekaController : ControllerBase
    {
        private readonly IYapayZekaService _yapayZekaService;

        public YapayZekaController(IYapayZekaService yapayZekaService)
        {
            _yapayZekaService = yapayZekaService;
        }

        [HttpPost("analiz")]
        public async Task<IActionResult> PortfoyYorumla([FromBody] PromptRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Soru))
            {
                return BadRequest(new { message = "Lütfen yapay zekaya sormak istediğiniz soruyu belirtin." });
            }

            // Eski Hali: string aiYaniti = await _yapayZekaService.PortfoyAnaliziYapAsync(request.Soru);
            // Yeni Hali:
            string aiYaniti = await _yapayZekaService.PortfoyAnaliziYapAsync(request.Soru, request.RiskProfili);

            return Ok(new
            {
                soru = request.Soru,
                yapayZekaYorumu = aiYaniti
            });
        }
    }

    public class PromptRequest
    {
        public string Soru { get; set; }

        // YENİ: Kullanıcının yatırım tarzını belirleyen alan
        // (Örn: "Garantici", "Dengeli", "Agresif")
        public string RiskProfili { get; set; } = "Dengeli"; // Varsayılan olarak Dengeli olsun
    }
}