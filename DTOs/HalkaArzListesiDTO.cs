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
        public int ToplamDagilacakLot { get; set; }
        public string? Sektor { get; set; }
        public List<int>? KonsorsiyumIds { get; set; }
        public List<string>? KonsorsiyumIsimleri { get; set; }
        public bool KatilimEndeksineUygunMu { get; set; }
        public int? GerceklesenKatilimciSayisi { get; set; }
        public double SektorOrtalamasi { get; set; }
        public double GenelPiyasaOrtalamasi { get; set; }
        public string? SirketOzeti { get; set; }
        public string? DagitimYontemi { get; set; }
        public decimal? ArzBuyukluguTL { get; set; }
        public decimal? HalkaAciklikOrani { get; set; }
        public decimal? IskontoOrani { get; set; }
        public string? FinansalCiroArtisi { get; set; }
        public string? FinansalKarMarji { get; set; }
        public string? FinansalBorcluluk { get; set; }
        public object? FonKullanimYerleri { get; set; }
        public List<string>? Taahhutler { get; set; }
        // CANLI BORSA VERİLERİ
        public decimal? GuncelFiyat { get; set; }
        public decimal? GunlukDegisimYuzdesi { get; set; }
        public List<decimal>? FiyatGecmisi { get; set; }
        public List<string>? ZamanGecmisi { get; set; } // YENİ
    }
}