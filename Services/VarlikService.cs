using PortfoyTakipAPI.DTOs;
using PortfoyTakipAPI.Models;
using PortfoyTakipAPI.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfoyTakipAPI.Services
{
    public class VarlikService : IVarlikService
    {
        private readonly IVarlikRepository _repository;

        public VarlikService(IVarlikRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<VarlikDTO> GetAll()
        {
            var varliklar = _repository.GetAll();

            return varliklar.Select(v => new VarlikDTO
            {
                Id = v.Id,
                Sembol = v.Sembol,
                Miktar = v.Miktar,
            }).ToList();
        }

        public async Task<PagedResult<VarlikDTO>> GetPagedVarliklarAsync(VarlikRequestParameters parameters)
        {
            var pagedData = await _repository.GetPagedVarliklarAsync(parameters);

            var dtoList = pagedData.Items.Select(v => new VarlikDTO
            {
                Id = v.Id,
                Sembol = v.Sembol,
                Miktar = v.Miktar,
            }).ToList();

            return new PagedResult<VarlikDTO>(dtoList, pagedData.TotalCount, pagedData.PageNumber, pagedData.PageSize);
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
            var yeniVarlik = new Varlik
            {
                Sembol = varlikDto.Sembol,
                VarlikTuru = varlikDto.VarlikTuru,
                Miktar = varlikDto.Miktar,
            };

            _repository.Add(yeniVarlik);
            _repository.Save();
        }
        public void Update(VarlikUpdateDTO varlikDto)
        {
            var mevcutVarlik = _repository.GetById(varlikDto.Id);

            if (mevcutVarlik != null)
            {
                mevcutVarlik.Sembol = varlikDto.Sembol;
                mevcutVarlik.VarlikTuru = varlikDto.VarlikTuru;
                mevcutVarlik.Miktar = varlikDto.Miktar;

                _repository.Update(mevcutVarlik);
                _repository.Save();
            }
        }
        public void Delete(int id)
        {
            _repository.Delete(id);
            _repository.Save();
        }
    }
}