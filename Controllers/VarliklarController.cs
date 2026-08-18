using Microsoft.AspNetCore.Mvc;
using PortfoyTakipAPI.DTOs;
using PortfoyTakipAPI.Services;

namespace PortfoyTakipAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VarliklarController : ControllerBase
    {
        private readonly IVarlikService _varlikService;

        // Dependency Injection: Garsona (Controller), Aşçıyı (Service) tanıtıyoruz
        public VarliklarController(IVarlikService varlikService)
        {
            _varlikService = varlikService;
        }

        // GET: api/varliklar
        [HttpGet]
        public IActionResult GetAll()
        {
            // Garson doğrudan aşçıdan (Service) veriyi istiyor
            var varliklar = _varlikService.GetAll();
            return Ok(varliklar);
        }

        // POST: api/varliklar
        [HttpPost]
        public IActionResult Add(VarlikCreateDTO varlikDto)
        {
            // Garson müşteriden gelen DTO tepsisini doğrudan aşçıya veriyor
            _varlikService.Add(varlikDto);
            return Ok("Yeni varlık portföye başarıyla eklendi.");
        }
    }
}