namespace PortfoyTakipAPI.Models
{
    public class HalkaArz
    {
        public int Id { get; set; }
        public string SirketAdi { get; set; }
        public decimal TalepFiyati { get; set; }
        public bool IslemGormeyeBasladiMi { get; set; }
    }
}
