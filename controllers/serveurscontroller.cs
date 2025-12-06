using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;

namespace RestaurantManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServeursController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServeursController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Serveurs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Serveur>>> GetServeurs()
        {
            return await _context.Serveurs
                .Include(s => s.Tables)
                .ToListAsync();
        }

        // GET: api/Serveurs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Serveur>> GetServeur(int id)
        {
            var serveur = await _context.Serveurs
                .Include(s => s.Tables)
                .FirstOrDefaultAsync(s => s.IdServeur == id);

            if (serveur == null)
                return NotFound();

            return serveur;
        }

        // POST: api/Serveurs
        [HttpPost]
        public async Task<ActionResult<Serveur>> PostServeur(Serveur serveur)
        {
            _context.Serveurs.Add(serveur);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetServeur), new { id = serveur.IdServeur }, serveur);
        }

        // PUT: api/Serveurs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServeur(int id, Serveur serveur)
        {
            if (id != serveur.IdServeur)
                return BadRequest();

            _context.Entry(serveur).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServeurExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Serveurs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServeur(int id)
        {
            var serveur = await _context.Serveurs.FindAsync(id);
            if (serveur == null)
                return NotFound();

            _context.Serveurs.Remove(serveur);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Serveurs/5/Tables
        [HttpGet("{id}/Tables")]
        public async Task<ActionResult<IEnumerable<Table>>> GetServeurTables(int id)
        {
            var serveur = await _context.Serveurs
                .Include(s => s.Tables)
                .FirstOrDefaultAsync(s => s.IdServeur == id);

            if (serveur == null)
                return NotFound();

            return Ok(serveur.Tables);
        }

        // POST: api/Serveurs/5/EncaisserPourboire
        [HttpPost("{id}/EncaisserPourboire")]
        public async Task<IActionResult> EncaisserPourboire(int id, [FromBody] double montant)
        {
            var serveur = await _context.Serveurs.FindAsync(id);
            if (serveur == null)
                return NotFound();

            serveur.Pourboires += montant;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Pourboire encaissé", nouveauTotal = serveur.Pourboires });
        }

        private bool ServeurExists(int id)
        {
            return _context.Serveurs.Any(e => e.IdServeur == id);
        }
    }
}
