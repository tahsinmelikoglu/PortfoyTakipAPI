using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
        private readonly IConfiguration _configuration;

        // appsettings.json dosyasını okumak için ana şaltere (Configuration) bağlanıyoruz
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] KullaniciGiris girisBilgileri)
        {
            // Şimdilik veritabanında kullanıcı tablomuz olmadığı için test amaçlı statik bir kontrol yapıyoruz.
            // (Sistem Yöneticisi test hesabı gibi düşün)
            if (girisBilgileri.KullaniciAdi == "admin" && girisBilgileri.Sifre == "123456")
            {
                // 1. Kimlik Kartı Bilgilerini Hazırla
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, girisBilgileri.KullaniciAdi),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                // 2. Senin o özel şifreni (1907Fenerbahce...) mühür (Key) olarak ayarla
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // 3. Bileti (Token) Bas
                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(1), // Bilet 1 saat geçerli
                    signingCredentials: creds
                );

                // Müşteriye Token'ı JSON olarak ver
                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }

            // Şifre yanlışsa kapıdan geri çevir
            return Unauthorized("Kullanıcı adı veya şifre hatalı.");
        }
    }
}