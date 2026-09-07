using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortfoyTakipAPI.Models
{
    public class HalkaArz
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string SirketAdi { get; set; }

        [Required]
        [MaxLength(10)]
        public string BorsaKodu { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LotFiyati { get; set; }

        // "Yaklaşan", "Talep Toplayan", "İşlem Gören", "Geçmiş"
        [Required]
        [MaxLength(30)]
        public string Statu { get; set; }

        public DateTime TalepToplamaBaslangic { get; set; }
        public DateTime TalepToplamaBitis { get; set; }

        public int ToplamDagilacakLot { get; set; }

        // --- MEVCUT VİZYONER ALANLAR ---
        [MaxLength(50)]
        public string? Sektor { get; set; }

        [MaxLength(100)]
        public string? KonsorsiyumLideri { get; set; }

        public bool KatilimEndeksineUygunMu { get; set; }

        public int? GerceklesenKatilimciSayisi { get; set; }

        // =======================================================
        // --- YENİ EKLENEN ARAYÜZ (FRONT-END) BESLEME ALANLARI ---
        // =======================================================

        [MaxLength(500)]
        public string? SirketOzeti { get; set; }

        [MaxLength(50)]
        public string? DagitimYontemi { get; set; } // Örn: "Bireysele Eşit Dağıtım"

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ArzBuyukluguTL { get; set; } // Örn: 1200000000 (1.2 Milyar TL)

        [Column(TypeName = "decimal(5,2)")]
        public decimal? HalkaAciklikOrani { get; set; } // Örn: 25.00

        [Column(TypeName = "decimal(5,2)")]
        public decimal? IskontoOrani { get; set; } // Örn: 15.00

        // --- FİNANSAL ÖZET BİLGİLERİ ---
        [MaxLength(50)]
        public string? FinansalCiroArtisi { get; set; } // Örn: "+%65" Veya "1.2 Milyar TL -> 1.8 Milyar TL"

        [MaxLength(50)]
        public string? FinansalKarMarji { get; set; } // Örn: "%22"

        [MaxLength(100)]
        public string? FinansalBorcluluk { get; set; } // Örn: "Düşük (Kaldıraç %30)"

        // --- KOLEKSİYONLAR (JSON OLARAK SAKLANACAK) ---
        // API'ye JSON Array string'i olarak gelip, DB'de string saklanacak.
        // EF Core 8/10 ile doğrudan JSON Column olarak yönetebiliriz.

        public string? FonKullanimYerleriJson { get; set; }
        // Örn: "[{\"alan\":\"GES Yatırımı\",\"oran\":70,\"renk\":\"success\"}]"

        public string? TaahhutlerJson { get; set; }
        // Örn: "[\"1 Yıl Ortak Satışı Yok\",\"30 Gün Fiyat İstikrarı\"]"
    }
}