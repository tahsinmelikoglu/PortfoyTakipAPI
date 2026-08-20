using Microsoft.EntityFrameworkCore;

namespace PortfoyTakipAPI.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Varlik> Varliklar { get; set; }
        public DbSet<HalkaArz> HalkaArzlar { get; set; }
        public DbSet<KullaniciGiris> Kullanicilar { get; set; }
    }
}