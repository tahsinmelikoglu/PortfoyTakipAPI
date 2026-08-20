namespace PortfoyTakipAPI.Models
{
    public class KullaniciGiris
    {
        public int Id { get; set; }
        public string KullaniciAdi { get; set; }
        public string Email { get; set; }
        public string SifreHash { get; set; }
    }
}