using Microsoft.EntityFrameworkCore;
using System;

namespace PortfoyTakipAPI.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Varlik> Varliklar { get; set; }
        public DbSet<HalkaArz> HalkaArzlar { get; set; }
        public DbSet<KullaniciGiris> Kullanicilar { get; set; }
        public DbSet<Konsorsiyum> Konsorsiyumlar { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}