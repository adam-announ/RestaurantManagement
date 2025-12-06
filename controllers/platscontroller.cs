using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;

namespace RestaurantManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlatsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Plat>>> GetPlats()
        {
            return await _context.Plats.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Plat>> GetPlat(int id)
        {
            var plat = await _context.Plats.FindAsync(id);

            if (plat == null)
                return NotFound();

            return plat;
        }

        [HttpPost]
        public async Task<ActionResult<Plat>> PostPlat(Plat plat)
        {
            _context.Plats.Add(plat);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlat), new { id = plat.IdPlat }, plat);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlat(int id, Plat plat)
        {
            if (id != plat.IdPlat)
                return BadRequest();

            _context.Entry(plat).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlat(int id)
        {
            var plat = await _context.Plats.FindAsync(id);
            if (plat == null)
                return NotFound();

            _context.Plats.Remove(plat);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("Disponibles")]
        public async Task<ActionResult<IEnumerable<Plat>>> GetPlatsDisponibles()
        {
            return await _context.Plats
                .Where(p => p.Disponible)
                .ToListAsync();
        }
    }
}
