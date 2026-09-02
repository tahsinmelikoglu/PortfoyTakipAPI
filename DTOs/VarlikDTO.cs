namespace PortfoyTakipAPI.DTOs
{
    public class VarlikDTO
    {
        public int Id { get; set; }
        public string Sembol { get; set; } // Örn: GLD, THYAO
        public decimal Miktar { get; set; }
        public decimal AlisFiyati { get; set; }
        public DateTime AlimTarihi { get; set; }

    }
}