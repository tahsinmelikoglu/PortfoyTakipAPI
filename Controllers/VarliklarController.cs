using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfoyTakipAPI.CQRS.Commands;
using PortfoyTakipAPI.CQRS.Queries;
using PortfoyTakipAPI.DTOs;
using PortfoyTakipAPI.Services;
using System.Security.Claims; // YENİ: JWT içindeki kimliği okumak için kütüphane eklendi
using System.Threading.Tasks;

namespace PortfoyTakipAPI.Controllers
{
    [Authorize] // Sınıf seviyesinde olduğu için tüm metotlar artık güvenli
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

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] VarlikRequestParameters parameters)
        {
            var query = new GetVarliklarQuery { Parameters = parameters };

            // YENİ: Token'dan kullanıcının ID'sini yakala ve Query'ye mühürle
            query.KullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateVarlikCommand command)
        {
            // YENİ: Dışarıdan ID gönderilse bile eziyoruz, Token'daki asıl sahibin ID'sini basıyoruz
            command.KullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var eklenenVarlik = await _mediator.Send(command);
            return Ok(eklenenVarlik);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] VarlikUpdateDTO varlikDto)
        {
            if (id != varlikDto.Id)
            {
                return BadRequest("URL'deki ID ile güncellenmek istenen verinin ID'si uyuşmuyor!");
            }

            // YENİ: Başkasının verisini güncelleyemesin diye işlemi yapanın ID'sini DTO'ya ekliyoruz
            varlikDto.KullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _varlikService.Update(varlikDto);
            return Ok("Varlık başarıyla güncellendi.");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Bu metot zaten sadece "Admin" rolüne açık olduğu için 
            // Admin herkesin verisini silebilsin diye KullaniciId kısıtlaması koymuyoruz.
            _varlikService.Delete(id);
            return Ok("Varlık portföyden başarıyla silindi.");
        }
    }
}