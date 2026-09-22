using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PortfoyTakipAPI.Models
{
    public class Konsorsiyum
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string KurumAdi { get; set; } // Örn: Ziraat Yatırım, Garanti BBVA

        // Çoka Çok (Many-to-Many) İlişki için Navigation Property
        [JsonIgnore] // Döngüsel hatayı önlemek için
        public ICollection<HalkaArz> HalkaArzlar { get; set; }
    }
}