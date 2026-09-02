using Microsoft.EntityFrameworkCore;
using System;

namespace PortfoyTakipAPI.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Varlik> Varliklar { get; set; }
        public DbSet<HalkaArz> HalkaArzlar { get; set; }
        public DbSet<KullaniciGiris> Kullanicilar { get; set; }
        public DbSet<ChatHistory> ChatHistories { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Halka Arz tablosuna varsayılan test ve gerçek dünya verilerini ekliyoruz
            modelBuilder.Entity<HalkaArz>().HasData(
                new HalkaArz
                {
                    Id = 1,
                    BorsaKodu = "MCARD",
                    SirketAdi = "Metropol Kurumsal Hizmetler",
                    LotFiyati = 25.00m,
                    ToplamDagilacakLot = 30000000,
                    Statu = "İşlem Gören",
                    TalepToplamaBaslangic = new DateTime(2026, 3, 10),
                    TalepToplamaBitis = new DateTime(2026, 3, 12)
                },
                new HalkaArz
                {
                    Id = 2,
                    BorsaKodu = "TKNJI",
                    SirketAdi = "Teknoloji Gelecek A.Ş.",
                    LotFiyati = 42.50m,
                    ToplamDagilacakLot = 15000000,
                    Statu = "Talep Toplayan",
                    TalepToplamaBaslangic = new DateTime(2026, 9, 1),
                    TalepToplamaBitis = new DateTime(2026, 9, 3)
                },
                new HalkaArz
                {
                    Id = 3,
                    BorsaKodu = "ENRGY",
                    SirketAdi = "Yeşil Enerji Üretim",
                    LotFiyati = 18.20m,
                    ToplamDagilacakLot = 55000000,
                    Statu = "Yaklaşan",
                    TalepToplamaBaslangic = new DateTime(2026, 9, 10),
                    TalepToplamaBitis = new DateTime(2026, 9, 12)
                }
            );
        }
    }
}