using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.Models;

namespace RestaurantAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployesController : ControllerBase
    {
        private readonly RestaurantContext _context;

        public EmployesController(RestaurantContext context)
        {
            _context = context;
        }

        // GET: api/employes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employe>>> GetEmployes()
        {
            return await _context.Employes
                .Include(e => e.Plannings)
                .ToListAsync();
        }

        // GET: api/employes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employe>> GetEmploye(int id)
        {
            var employe = await _context.Employes
                .Include(e => e.Plannings)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employe == null)
                return NotFound();

            return employe;
        }

        // GET: api/employes/serveurs
        [HttpGet("serveurs")]
        public async Task<ActionResult<IEnumerable<Serveur>>> GetServeurs()
        {
            return await _context.Serveurs
                .Include(s => s.Commandes)
                .ToListAsync();
        }

        // GET: api/employes/cuisiniers
        [HttpGet("cuisiniers")]
        public async Task<ActionResult<IEnumerable<Cuisinier>>> GetCuisiniers()
        {
            return await _context.Cuisiniers
                .Include(c => c.CommandesPreparees)
                .ToListAsync();
        }

        // GET: api/employes/managers
        [HttpGet("managers")]
        public async Task<ActionResult<IEnumerable<Manager>>> GetManagers()
        {
            return await _context.Managers.ToListAsync();
        }

        // POST: api/employes/serveur
        [HttpPost("serveur")]
        public async Task<ActionResult<Serveur>> CreateServeur(Serveur serveur)
        {
            serveur.DateEmbauche = DateTime.UtcNow;
            _context.Serveurs.Add(serveur);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmploye), new { id = serveur.Id }, serveur);
        }

        // POST: api/employes/cuisinier
        [HttpPost("cuisinier")]
        public async Task<ActionResult<Cuisinier>> CreateCuisinier(Cuisinier cuisinier)
        {
            cuisinier.DateEmbauche = DateTime.UtcNow;
            _context.Cuisiniers.Add(cuisinier);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmploye), new { id = cuisinier.Id }, cuisinier);
        }

        // POST: api/employes/manager
        [HttpPost("manager")]
        public async Task<ActionResult<Manager>> CreateManager(Manager manager)
        {
            manager.DateEmbauche = DateTime.UtcNow;
            _context.Managers.Add(manager);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmploye), new { id = manager.Id }, manager);
        }

        // PUT: api/employes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmploye(int id, Employe employe)
        {
            if (id != employe.Id)
                return BadRequest();

            _context.Entry(employe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Employes.AnyAsync(e => e.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/employes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmploye(int id)
        {
            var employe = await _context.Employes.FindAsync(id);
            
            if (employe == null)
                return NotFound();

            _context.Employes.Remove(employe);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/employes/5/planning
        [HttpGet("{id}/planning")]
        public async Task<ActionResult<IEnumerable<Planning>>> GetEmployePlanning(int id)
        {
            var employe = await _context.Employes.FindAsync(id);
            
            if (employe == null)
                return NotFound();

            var plannings = await _context.Plannings
                .Where(p => p.EmployeId == id)
                .OrderBy(p => p.Date)
                .ToListAsync();

            return plannings;
        }
    }
}