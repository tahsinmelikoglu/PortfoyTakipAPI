namespace PortfoyTakipAPI.DTOs
{
    public class HalkaArzListesiDTO
    {
        public int Id { get; set; }
        public string SirketAdi { get; set; }
        public string BorsaKodu { get; set; }
        public decimal LotFiyati { get; set; }
        public string Statu { get; set; }

        // Frontend'de rahat göstermek için tarihleri direkt string'e çevirip yolluyoruz
        public string TalepToplamaBaslangic { get; set; }
        public string TalepToplamaBitis { get; set; }

        public string? Sektor { get; set; }
        public string? KonsorsiyumLideri { get; set; }
        public bool KatilimEndeksineUygunMu { get; set; }
        public int? GerceklesenKatilimciSayisi { get; set; }

        // --- YENİ EKLENEN ANALİTİK ALANLAR ---
        public double SektorOrtalamasi { get; set; }
        public double GenelPiyasaOrtalamasi { get; set; }
    }
}