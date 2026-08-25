using System.Collections.Generic;
using System.Threading.Tasks;
using PortfoyTakipAPI.Models;
using PortfoyTakipAPI.DTOs;

namespace PortfoyTakipAPI.Repositories
{
    public interface IVarlikRepository
    {
        IEnumerable<Varlik> GetAll();
        Task<PagedResult<Varlik>> GetPagedVarliklarAsync(VarlikRequestParameters parameters);
        Varlik GetById(int id);
        void Add(Varlik varlik);
        void Update(Varlik varlik);
        void Delete(int id);
        void Save();                   
    }
}