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
    }
}
