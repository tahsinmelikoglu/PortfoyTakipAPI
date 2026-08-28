using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfoyTakipAPI.CQRS.Commands;
using PortfoyTakipAPI.CQRS.Queries; // YENİ: Queries klasörünü dahil ettik
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
        private readonly IMediator _mediator;

        public VarliklarController(IVarlikService varlikService, IMediator mediator)
        {
            _varlikService = varlikService;
            _mediator = mediator;
        }

        // =======================================================
        // YENİ: CQRS ve MediatR ile çalışan Modern GetAll Metodu
        // =======================================================
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] VarlikRequestParameters parameters)
        {
            // Parametreleri pakete koyup MediatR'a fırlatıyoruz
            var query = new GetVarliklarQuery { Parameters = parameters };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateVarlikCommand command)
        {
            var eklenenVarlik = await _mediator.Send(command);
            return Ok(eklenenVarlik);
        }

        // Kalan Update ve Delete metotları şimdilik eski sistemle çalışmaya devam ediyor
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