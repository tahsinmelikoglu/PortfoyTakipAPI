namespace PortfoyTakipAPI.DTOs
{
    public class VarlikCreateDTO
    {
        public string Sembol { get; set; }
        public string VarlikTuru { get; set; }
        public decimal Miktar { get; set; }
        public decimal AlisFiyati { get; set; }
        public DateTime AlimTarihi { get; set; }
    }
}