using PortfoyTakipAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortfoyTakipAPI.Services
{
    public interface IVarlikService
    {
        IEnumerable<VarlikDTO> GetAll();


        Task<PagedResult<VarlikDTO>> GetPagedVarliklarAsync(VarlikRequestParameters parameters);

        VarlikDTO GetById(int id);
        void Add(VarlikCreateDTO varlikDto);
        void Update(VarlikUpdateDTO varlikDto);
        void Delete(int id);
    }
}