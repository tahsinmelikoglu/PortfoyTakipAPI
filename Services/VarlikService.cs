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
        public void Update(VarlikUpdateDTO varlikDto)
        {
            // 1. Kilerdeki (Veritabanındaki) mevcut varlığı bul
            var mevcutVarlik = _repository.GetById(varlikDto.Id);

            // Eğer varlık gerçekten varsa güncelleme yap
            if (mevcutVarlik != null)
            {
                // 2. Tepsideki yeni bilgileri mevcut kaydın üzerine yaz
                mevcutVarlik.Sembol = varlikDto.Sembol;
                mevcutVarlik.VarlikTuru = varlikDto.VarlikTuru;
                mevcutVarlik.Miktar = varlikDto.Miktar;

                // 3. Kiler sorumlusuna (Repository) güncellemeyi bildir ve kaydet
                _repository.Update(mevcutVarlik);
                _repository.Save();
            }
        }
        public void Delete(int id)
        {
            // Kiler sorumlusuna (Repository) doğrudan silme emrini veriyoruz
            _repository.Delete(id);
            _repository.Save();
        }
    }
}