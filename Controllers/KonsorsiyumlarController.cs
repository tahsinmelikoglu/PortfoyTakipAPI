using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfoyTakipAPI.Models;
using System.Threading.Tasks;

namespace PortfoyTakipAPI.Controllers
{
    // YENİ: Sadece dışarıdan gelen veriyi karşılayacak minik DTO sınıfımız
    public class YeniKurumDto
    {
        public string KurumAdi { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class KonsorsiyumlarController : ControllerBase
    {
        private readonly AppDbContext _context;

        public KonsorsiyumlarController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Konsorsiyumlar
        [HttpGet]
        public async Task<IActionResult> GetBankalar()
        {
            var bankalar = await _context.Konsorsiyumlar
                                         .AsNoTracking()
                                         .ToListAsync();

            return Ok(bankalar);
        }

        // POST: api/Konsorsiyumlar
        [HttpPost]
        [Authorize] // Sadece yetkili kullanıcılar kurum ekleyebilir
        public async Task<IActionResult> YeniBankaEkle([FromBody] YeniKurumDto yeniKurum) // <-- Doğrudan Konsorsiyum yerine DTO alıyoruz
        {
            if (string.IsNullOrWhiteSpace(yeniKurum.KurumAdi))
                return BadRequest("Banka adı zorunludur.");

            // DTO'dan gelen veriyi gerçek veritabanı nesnesine dönüştürüyoruz
            var banka = new Konsorsiyum
            {
                KurumAdi = yeniKurum.KurumAdi.Trim()
            };

            _context.Konsorsiyumlar.Add(banka);
            await _context.SaveChangesAsync();

            return Ok(banka);
        }
    }
}