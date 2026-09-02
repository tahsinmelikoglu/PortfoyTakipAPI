using System.Text.Json.Serialization;

namespace PortfoyTakipAPI.DTOs
{
    public class KullaniciLoginDTO
    {
        [JsonPropertyName("kullaniciAdi")]
        public string KullaniciAdi { get; set; } = string.Empty;

        [JsonPropertyName("sifre")]
        public string Sifre { get; set; } = string.Empty;
    }
}