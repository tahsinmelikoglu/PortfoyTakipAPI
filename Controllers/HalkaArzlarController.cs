using MediatR;
using Microsoft.AspNetCore.Mvc;
using PortfoyTakipAPI.CQRS.Queries;
using PortfoyTakipAPI.DTOs;
using System.Threading.Tasks;

namespace PortfoyTakipAPI.Controllers
{
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
    }
}