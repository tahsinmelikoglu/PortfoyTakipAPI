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
        // PUT: api/varliklar/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, VarlikUpdateDTO varlikDto)
        {
            // Güvenlik ve Tutarlılık Kontrolü: 
            // Adres çubuğundan gönderilen ID ile DTO tepsisindeki ID aynı mı?
            if (id != varlikDto.Id)
            {
                return BadRequest("URL'deki ID ile güncellenmek istenen verinin ID'si uyuşmuyor!");
            }

            // Garson, DTO tepsisini doğrudan aşçıya (Service) veriyor
            _varlikService.Update(varlikDto);

            return Ok("Varlık başarıyla güncellendi.");
        }
        // DELETE: api/varliklar/6
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Garson, silinecek ID'yi alıp aşçıya iletiyor
            _varlikService.Delete(id);
            return Ok("Varlık portföyden başarıyla silindi.");
        }

    }
}