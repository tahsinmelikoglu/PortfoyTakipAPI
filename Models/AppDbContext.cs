using Microsoft.EntityFrameworkCore;

namespace PortfoyTakipAPI.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Varlik> Varliklar { get; set; }
        public DbSet<HalkaArz> HalkaArzlar { get; set; }
        public DbSet<KullaniciGiris> Kullanicilar { get; set; }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // Projedeki tüm decimal (ondalıklı) sayılar için varsayılan limiti belirliyoruz
            configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
        }
    }
}