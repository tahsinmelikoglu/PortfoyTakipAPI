using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfoyTakipAPI.DTOs;
using PortfoyTakipAPI.Services;
using System.Threading.Tasks;

namespace PortfoyTakipAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VarliklarController : ControllerBase
    {
        private readonly IVarlikService _varlikService;

        public VarliklarController(IVarlikService varlikService)
        {
            _varlikService = varlikService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] VarlikRequestParameters parameters)
        {
            var result = await _varlikService.GetPagedVarliklarAsync(parameters);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Add(VarlikCreateDTO varlikDto)
        {
            _varlikService.Add(varlikDto);
            return Ok("Yeni varlık portföye başarıyla eklendi.");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, VarlikUpdateDTO varlikDto)
        {
            if (id != varlikDto.Id)
            {
                return BadRequest("URL'deki ID ile güncellenmek istenen verinin ID'si uyuşmuyor!");
            }

            _varlikService.Update(varlikDto);

            return Ok("Varlık başarıyla güncellendi.");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _varlikService.Delete(id);
            return Ok("Varlık portföyden başarıyla silindi.");
        }
    }
}