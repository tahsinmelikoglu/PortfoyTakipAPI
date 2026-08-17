using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfoyTakipAPI.Models;
using System.Formats.Asn1;
using Microsoft.AspNetCore.Authorization;

namespace PortfoyTakipAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
        
    public class VarliklarController : ControllerBase
    {
        private readonly AppDbContext _context;
        public VarliklarController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
    public async Task<IActionResult> GetVarliklar()
        {
            var varliklar = await _context.Varliklar.ToListAsync();
            return Ok(varliklar);
        }
        [HttpPost]
        public async Task<IActionResult> VarlikEkle([FromBody] Varlik yeniVarlik)
        {
            
            _context.Varliklar.Add(yeniVarlik);
           
            await _context.SaveChangesAsync();

            return Ok(yeniVarlik);
        }
    }

}
