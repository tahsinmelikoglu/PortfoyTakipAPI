namespace PortfoyTakipAPI.DTOs
{
    public class SimulasyonRequestDTO
    {
        public int AlinanLotAdedi { get; set; }
        public decimal HalkaArzFiyati { get; set; }
        public decimal HedefKarMarjiYuzdesi { get; set; } // Örn: 100 (yani %100 tavan serisi)
    }

    public class SimulasyonResponseDTO
    {
        public decimal ToplamYatirilanTutar { get; set; }
        public decimal HedefFiyat { get; set; }
        public int SatilacakLotAdedi { get; set; }
        public int IcerideKalanLotAdedi { get; set; }
        public decimal IcerideKalanLotlarinDegeri { get; set; }
        public string StratejiAciklamasi { get; set; }
    }
}