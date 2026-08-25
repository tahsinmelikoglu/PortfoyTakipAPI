using Microsoft.EntityFrameworkCore;
using PortfoyTakipAPI.DTOs;
using PortfoyTakipAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        // --- YENİ EKLENEN SAYFALAMA VE FİLTRELEME METODU ---
        public async Task<PagedResult<Varlik>> GetPagedVarliklarAsync(VarlikRequestParameters parameters)
        {
            var query = _context.Varliklar.AsQueryable();

            // 1. Filtreleme (Sembol alanında arama yapar)
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                query = query.Where(v => v.Sembol.Contains(parameters.SearchTerm));
            }

            // 2. Sıralama
            query = parameters.IsDescending
                ? query.OrderByDescending(v => v.Id)
                : query.OrderBy(v => v.Id);

            // 3. Sayfalama ve Toplam Kayıt Sayısı
            var totalCount = await query.CountAsync();
            var items = await query.Skip((parameters.PageNumber - 1) * parameters.PageSize)
                                   .Take(parameters.PageSize)
                                   .ToListAsync();

            return new PagedResult<Varlik>(items, totalCount, parameters.PageNumber, parameters.PageSize);
        }
        // ----------------------------------------------------

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