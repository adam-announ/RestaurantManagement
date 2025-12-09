using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.Models;

namespace RestaurantAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatsController : ControllerBase
    {
        private readonly RestaurantContext _context;

        public PlatsController(RestaurantContext context)
        {
            _context = context;
        }

        // GET: api/plats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Plat>>> GetPlats()
        {
            return await _context.Plats
                .Include(p => p.Ingredients)
                .ToListAsync();
        }

        // GET: api/plats/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Plat>> GetPlat(int id)
        {
            var plat = await _context.Plats
                .Include(p => p.Ingredients)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (plat == null)
                return NotFound();

            return plat;
        }

        // GET: api/plats/disponibles
        [HttpGet("disponibles")]
        public async Task<ActionResult<IEnumerable<Plat>>> GetPlatsDisponibles()
        {
            return await _context.Plats
                .Where(p => p.Disponible)
                .Include(p => p.Ingredients)
                .ToListAsync();
        }

        // GET: api/plats/categorie/{categorie}
        [HttpGet("categorie/{categorie}")]
        public async Task<ActionResult<IEnumerable<Plat>>> GetPlatsByCategorie(string categorie)
        {
            return await _context.Plats
                .Where(p => p.Categorie == categorie)
                .ToListAsync();
        }

        // POST: api/plats
        [HttpPost]
        public async Task<ActionResult<Plat>> CreatePlat(Plat plat)
        {
            _context.Plats.Add(plat);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlat), new { id = plat.Id }, plat);
        }

        // PUT: api/plats/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlat(int id, Plat plat)
        {
            if (id != plat.Id)
                return BadRequest();

            _context.Entry(plat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Plats.AnyAsync(p => p.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // PATCH: api/plats/5/disponibilite
        [HttpPatch("{id}/disponibilite")]
        public async Task<IActionResult> UpdateDisponibilite(int id, [FromBody] bool disponible)
        {
            var plat = await _context.Plats.FindAsync(id);
            
            if (plat == null)
                return NotFound();

            plat.Disponible = disponible;
            await _context.SaveChangesAsync();

            return Ok(plat);
        }

        // POST: api/plats/5/ingredients
        [HttpPost("{id}/ingredients")]
        public async Task<IActionResult> AddIngredient(int id, [FromBody] int ingredientId)
        {
            var plat = await _context.Plats
                .Include(p => p.Ingredients)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            if (plat == null)
                return NotFound("Plat non trouvé");

            var ingredient = await _context.Ingredients.FindAsync(ingredientId);
            
            if (ingredient == null)
                return NotFound("Ingrédient non trouvé");

            plat.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();

            return Ok(plat);
        }

        // DELETE: api/plats/5
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
    }
}