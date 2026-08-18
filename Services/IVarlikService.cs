using PortfoyTakipAPI.DTOs;

namespace PortfoyTakipAPI.Services
{
    public interface IVarlikService
    {
        IEnumerable<VarlikDTO> GetAll();
        VarlikDTO GetById(int id);
        void Add(VarlikCreateDTO varlikDto);
    }
}