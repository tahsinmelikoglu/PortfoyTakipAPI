using System;
using System.ComponentModel.DataAnnotations;

namespace PortfoyTakipAPI.Models // Kendi namespace'ine göre ayarla
{
    public class HalkaArz
    {
        public int Id { get; set; }
        public string SirketAdi { get; set; }
        public string BorsaKodu { get; set; }
        public decimal LotFiyati { get; set; }

        // "Yaklaşan", "Talep Toplayan", "İşlem Gören", "Geçmiş"
        public string Statu { get; set; }

        public DateTime TalepToplamaBaslangic { get; set; }
        public DateTime TalepToplamaBitis { get; set; }
        public int ToplamDagilacakLot { get; set; }

        // --- YENİ EKLENEN VİZYONER ALANLAR ---

        // Örn: "Teknoloji", "Enerji", "Gayrimenkul", "Gıda"
        public string? Sektor { get; set; }

        // Örn: "İş Yatırım", "Garanti BBVA"
        public string? KonsorsiyumLideri { get; set; }

        // Hassas yatırımcılar için kritik bir filtre
        public bool KatilimEndeksineUygunMu { get; set; }

        // Sektörel tahminde kullanılacak olan GEÇMİŞ arzların gerçek katılım sayısı
        // Yaklaşan arzlar için bu değer "null" veya "0" olabilir.
        public int? GerceklesenKatilimciSayisi { get; set; }
    }
}