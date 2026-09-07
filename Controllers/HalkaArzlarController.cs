using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfoyTakipAPI.CQRS.Queries;
using PortfoyTakipAPI.DTOs;
using System.Threading.Tasks;

namespace PortfoyTakipAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class HalkaArzlarController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HalkaArzlarController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // 1. GET METODU: Halka arz listesini getirir (Az önce yazdığımız Query)
        // İstersek api/HalkaArzlar?statu=Yaklaşan şeklinde filtreleme de yapabiliriz
        [HttpGet]
        public async Task<IActionResult> GetHalkaArzlar([FromQuery] string statu = null)
        {
            var query = new GetHalkaArzlarQuery { StatuFiltresi = statu };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        // 2. POST METODU: Kâr Al / İçeride Bırak stratejisini hesaplar
        [HttpPost("simulasyon")]
        public async Task<IActionResult> SimulasyonHesapla([FromBody] SimulasyonRequestDTO request)
        {
            var query = new CalculateHalkaArzSimulasyonQuery { Parameters = request };
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateHalkaArz([FromBody] PortfoyTakipAPI.CQRS.Commands.CreateHalkaArzCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { Message = "Halka Arz başarıyla oluşturuldu.", Id = id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHalkaArz(int id, [FromBody] PortfoyTakipAPI.CQRS.Commands.UpdateHalkaArzCommand command)
        {
            // URL'deki Id ile Body'den gelen Id uyuşuyor mu güvenlik kontrolü
            if (id != command.Id)
            {
                return BadRequest(new { Message = "URL'deki ID ile gönderilen verinin ID'si uyuşmuyor." });
            }

            var result = await _mediator.Send(command);

            if (!result)
            {
                return NotFound(new { Message = "Güncellenecek kayıt bulunamadı." });
            }

            return Ok(new { Message = "Halka arz başarıyla güncellendi." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHalkaArz(int id)
        {
            var command = new PortfoyTakipAPI.CQRS.Commands.DeleteHalkaArzCommand { Id = id };
            var result = await _mediator.Send(command);

            if (!result)
            {
                return NotFound(new { Message = "Silinecek kayıt bulunamadı." });
            }

            return Ok(new { Message = "Halka Arz başarıyla silindi." });
        }
    }
}