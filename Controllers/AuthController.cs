using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PortfoyTakipAPI.DTOs;
using PortfoyTakipAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
            if (_context.Kullanicilar.Any(u => u.KullaniciAdi == request.KullaniciAdi))
            {
                return BadRequest("Bu kullanıcı adı zaten alınmış.");
            }

            var yeniKullanici = new KullaniciGiris
            {
                KullaniciAdi = request.KullaniciAdi,
                Email = request.Email,
                SifreHash = BCrypt.Net.BCrypt.HashPassword(request.Sifre),
                Rol = "User" // Varsayılan rol ataması
            };

            _context.Kullanicilar.Add(yeniKullanici);
            _context.SaveChanges();

            return Ok("Kullanıcı kaydı başarıyla oluşturuldu.");
        }

        [HttpPost("login")]
        public IActionResult Login(KullaniciLoginDTO request)
        {
            var kullanici = _context.Kullanicilar.FirstOrDefault(u => u.KullaniciAdi == request.KullaniciAdi);

            if (kullanici == null || !BCrypt.Net.BCrypt.Verify(request.Sifre, kullanici.SifreHash))
            {
                return BadRequest("Kullanıcı adı veya şifre hatalı.");
            }

            // Access ve Refresh Token Üretimi
            string accessToken = CreateToken(kullanici);
            string refreshToken = GenerateRefreshToken();

            // Refresh Token'ı veritabanına kaydet (Ömrü 7 gün)
            kullanici.RefreshToken = refreshToken;
            kullanici.RefreshTokenBitisSuresi = DateTime.Now.AddDays(7);
            _context.SaveChanges();

            return Ok(new TokenModelDTO { AccessToken = accessToken, RefreshToken = refreshToken });
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken(TokenModelDTO request)
        {
            // Gelen refresh token'a sahip kullanıcıyı bul
            var kullanici = _context.Kullanicilar.FirstOrDefault(u => u.RefreshToken == request.RefreshToken);

            // Kullanıcı yoksa veya Refresh Token'ın süresi dolmuşsa
            if (kullanici == null || kullanici.RefreshTokenBitisSuresi < DateTime.Now)
            {
                return Unauthorized("Geçersiz veya süresi dolmuş Refresh Token. Lütfen tekrar giriş yapın.");
            }

            // Her şey yolundaysa yeni token ikilisini üret
            string yeniAccessToken = CreateToken(kullanici);
            string yeniRefreshToken = GenerateRefreshToken();

            // Veritabanını yeni Refresh Token ile güncelle
            kullanici.RefreshToken = yeniRefreshToken;
            kullanici.RefreshTokenBitisSuresi = DateTime.Now.AddDays(7);
            _context.SaveChanges();

            return Ok(new TokenModelDTO { AccessToken = yeniAccessToken, RefreshToken = yeniRefreshToken });
        }

        private string CreateToken(KullaniciGiris kullanici)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, kullanici.Id.ToString()),
                new Claim(ClaimTypes.Name, kullanici.KullaniciAdi),
                new Claim(ClaimTypes.Role, kullanici.Rol) // ROL BİLGİSİ TOKEN'A EKLENDİ
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                issuer: _configuration.GetSection("Jwt:Issuer").Value,
                audience: _configuration.GetSection("Jwt:Audience").Value,
                claims: claims,
                expires: DateTime.Now.AddMinutes(15), // ACCESS TOKEN ÖMRÜ 15 DAKİKAYA DÜŞÜRÜLDÜ
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            // Rastgele, güçlü bir 64 bytelık metin üretir
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}