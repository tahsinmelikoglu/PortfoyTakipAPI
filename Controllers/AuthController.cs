using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PortfoyTakipAPI.DTOs;
using PortfoyTakipAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PortfoyTakipAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register(KullaniciRegisterDTO request)
        {
            // Kullanıcı adı daha önce alınmış mı kontrolü
            if (_context.Kullanicilar.Any(u => u.KullaniciAdi == request.KullaniciAdi))
            {
                return BadRequest("Bu kullanıcı adı zaten alınmış.");
            }

            // Şifreyi BCrypt ile hashleyip veritabanına kaydediyoruz
            var yeniKullanici = new KullaniciGiris
            {
                KullaniciAdi = request.KullaniciAdi,
                Email = request.Email,
                SifreHash = BCrypt.Net.BCrypt.HashPassword(request.Sifre)
            };

            _context.Kullanicilar.Add(yeniKullanici);
            _context.SaveChanges();

            return Ok("Kullanıcı kaydı başarıyla oluşturuldu.");
        }

        [HttpPost("login")]
        public IActionResult Login(KullaniciLoginDTO request)
        {
            // Kullanıcıyı bul
            var kullanici = _context.Kullanicilar.FirstOrDefault(u => u.KullaniciAdi == request.KullaniciAdi);

            // Kullanıcı yoksa veya Hashlenmiş şifreler eşleşmiyorsa
            if (kullanici == null || !BCrypt.Net.BCrypt.Verify(request.Sifre, kullanici.SifreHash))
            {
                return BadRequest("Kullanıcı adı veya şifre hatalı.");
            }

            // Doğrulama başarılıysa Token üret
            string token = CreateToken(kullanici);

            return Ok(new { Token = token });
        }

        private string CreateToken(KullaniciGiris kullanici)
        {
            // Token içine gömülecek kullanıcı bilgileri (Claims)
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, kullanici.Id.ToString()),
                new Claim(ClaimTypes.Name, kullanici.KullaniciAdi)
            };

            // appsettings.json içindeki gizli anahtarımızı alıyoruz
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                issuer: _configuration.GetSection("Jwt:Issuer").Value,
                audience: _configuration.GetSection("Jwt:Audience").Value,
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}