using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortfoyTakipAPI.Models
{
    public class Varlik
    {
        public int Id { get; set; }
        public string VarlikTuru { get; set; }
        public string Sembol { get; set; }
        public decimal Miktar { get; set; }
        public decimal Bakiye { get; set; }
        public decimal AlisFiyati { get; set; }
        public DateTime AlimTarihi { get; set; }
        [NotMapped]
        public decimal GuncelFiyat { get; set; }

        [Required(ErrorMessage = "Kullanıcı ID alanı boş bırakılamaz!")]
        public string KullaniciId { get; set; }

    }
}
