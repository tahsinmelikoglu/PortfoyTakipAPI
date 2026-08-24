namespace PortfoyTakipAPI.Models
{
    public class KullaniciGiris
    {
        public int Id { get; set; }
        public string KullaniciAdi { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SifreHash { get; set; } = string.Empty;
        public string Rol { get; set; } = "User";
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenBitisSuresi { get; set; }
    }
}