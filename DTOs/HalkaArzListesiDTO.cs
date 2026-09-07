namespace PortfoyTakipAPI.DTOs
{
    public class HalkaArzListesiDTO
    {
        public int Id { get; set; }
        public string SirketAdi { get; set; }
        public string BorsaKodu { get; set; }
        public decimal LotFiyati { get; set; }
        public string Statu { get; set; }
        public string TalepToplamaBaslangic { get; set; }
        public string TalepToplamaBitis { get; set; }
        public int ToplamDagilacakLot { get; set; } // BU SATIRI EKLE
        public string? Sektor { get; set; }
        public string? KonsorsiyumLideri { get; set; }
        public bool KatilimEndeksineUygunMu { get; set; }
        public int? GerceklesenKatilimciSayisi { get; set; }

        public double SektorOrtalamasi { get; set; }
        public double GenelPiyasaOrtalamasi { get; set; }

        // --- YENİ EKLENEN VİZYONER ALANLAR ---
        public string? SirketOzeti { get; set; }
        public string? DagitimYontemi { get; set; }
        public decimal? ArzBuyukluguTL { get; set; }
        public decimal? HalkaAciklikOrani { get; set; }
        public decimal? IskontoOrani { get; set; }
        public string? FinansalCiroArtisi { get; set; }
        public string? FinansalKarMarji { get; set; }
        public string? FinansalBorcluluk { get; set; }

        // JSON'ları nesne olarak dışarıya verebilmek için
        public object? FonKullanimYerleri { get; set; }
        public List<string>? Taahhutler { get; set; }
    }
}