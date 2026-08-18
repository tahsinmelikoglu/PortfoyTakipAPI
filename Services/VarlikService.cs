using PortfoyTakipAPI.DTOs;
using PortfoyTakipAPI.Models;
using PortfoyTakipAPI.Repositories;

namespace PortfoyTakipAPI.Services
{
    public class VarlikService : IVarlikService
    {
        private readonly IVarlikRepository _repository;

        // Dependency Injection ile Kiler sorumlusunu Aşçıya veriyoruz
        public VarlikService(IVarlikRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<VarlikDTO> GetAll()
        {
            var varliklar = _repository.GetAll();

            // Veritabanından gelen ham verileri DTO tepsisine diziyoruz
            return varliklar.Select(v => new VarlikDTO
            {
                Id = v.Id,
                Sembol = v.Sembol,
                Miktar = v.Miktar,
            }).ToList();
        }

        public VarlikDTO GetById(int id)
        {
            var v = _repository.GetById(id);
            if (v == null) return null;

            return new VarlikDTO
            {
                Id = v.Id,
                Sembol = v.Sembol,
                Miktar = v.Miktar,
            };
        }

        public void Add(VarlikCreateDTO varlikDto)
        {
            // Dışarıdan gelen Id'siz DTO'yu, veritabanına yazılacak formata çeviriyoruz
            var yeniVarlik = new Varlik
            {
                Sembol = varlikDto.Sembol,
                VarlikTuru = varlikDto.VarlikTuru,
                Miktar = varlikDto.Miktar,
            };

            _repository.Add(yeniVarlik);
            _repository.Save(); // Kaydet emrini kiler sorumlusuna iletiyoruz
        }
    }
}