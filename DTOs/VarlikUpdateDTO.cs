namespace PortfoyTakipAPI.DTOs
{
    public class VarlikUpdateDTO
    {

        public int Id { get; set; }
        public string KullaniciId { get; set; }
        public string Sembol { get; set; }
        public string VarlikTuru { get; set; }
        public decimal Miktar { get; set; }
        public decimal AlisFiyati { get; set; }
        public DateTime AlimTarihi { get; set; }
    }
}
