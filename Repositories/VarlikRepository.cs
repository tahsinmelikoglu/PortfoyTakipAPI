using PortfoyTakipAPI.Models;

namespace PortfoyTakipAPI.Repositories
{
    // Sınıfın IVarlikRepository sözleşmesini uygulayacağını belirtiyoruz (Miras)
    public class VarlikRepository : IVarlikRepository
    {
        private readonly AppDbContext _context;

        // Dependency Injection (Bağımlılık Enjeksiyonu) ile Veritabanını alıyoruz
        public VarlikRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Varlik> GetAll()
        {
            return _context.Varliklar.ToList();
        }

        public Varlik GetById(int id)
        {
            return _context.Varliklar.Find(id);
        }

        public void Add(Varlik varlik)
        {
            _context.Varliklar.Add(varlik);
        }

        public void Update(Varlik varlik)
        {
            _context.Varliklar.Update(varlik);
        }

        public void Delete(int id)
        {
            var varlik = _context.Varliklar.Find(id);
            if (varlik != null)
            {
                _context.Varliklar.Remove(varlik);
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}