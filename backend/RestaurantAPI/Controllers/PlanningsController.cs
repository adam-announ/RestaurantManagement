using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.Models;

namespace RestaurantAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanningsController : ControllerBase
    {
        private readonly RestaurantContext _context;

        public PlanningsController(RestaurantContext context)
        {
            _context = context;
        }

        // GET: api/plannings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Planning>>> GetPlannings()
        {
            return await _context.Plannings
                .Include(p => p.Employe)
                .OrderBy(p => p.Date)
                .ToListAsync();
        }

        // GET: api/plannings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Planning>> GetPlanning(int id)
        {
            var planning = await _context.Plannings
                .Include(p => p.Employe)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (planning == null)
                return NotFound();

            return planning;
        }

        // GET: api/plannings/date/2024-01-15
        [HttpGet("date/{date}")]
        public async Task<ActionResult<IEnumerable<Planning>>> GetPlanningsByDate(DateTime date)
        {
            return await _context.Plannings
                .Where(p => p.Date.Date == date.Date)
                .Include(p => p.Employe)
                .OrderBy(p => p.HeureDebut)
                .ToListAsync();
        }

        // GET: api/plannings/semaine/{date}
        [HttpGet("semaine/{date}")]
        public async Task<ActionResult<IEnumerable<Planning>>> GetPlanningsSemaine(DateTime date)
        {
            var debutSemaine = date.Date.AddDays(-(int)date.DayOfWeek + 1);
            var finSemaine = debutSemaine.AddDays(7);

            return await _context.Plannings
                .Where(p => p.Date >= debutSemaine && p.Date < finSemaine)
                .Include(p => p.Employe)
                .OrderBy(p => p.Date)
                .ThenBy(p => p.HeureDebut)
                .ToListAsync();
        }

        // POST: api/plannings
        [HttpPost]
        public async Task<ActionResult<Planning>> CreatePlanning(Planning planning)
        {
            // Vérifier si l'employé existe
            var employe = await _context.Employes.FindAsync(planning.EmployeId);
            if (employe == null)
                return NotFound("Employé non trouvé");

            // Vérifier conflit d'horaire
            var conflit = await _context.Plannings
                .AnyAsync(p => p.EmployeId == planning.EmployeId
                    && p.Date.Date == planning.Date.Date
                    && ((planning.HeureDebut >= p.HeureDebut && planning.HeureDebut < p.HeureFin)
                        || (planning.HeureFin > p.HeureDebut && planning.HeureFin <= p.HeureFin)));

            if (conflit)
                return BadRequest("Conflit d'horaire pour cet employé");

            _context.Plannings.Add(planning);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlanning), new { id = planning.Id }, planning);
        }

        // PUT: api/plannings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlanning(int id, Planning planning)
        {
            if (id != planning.Id)
                return BadRequest();

            _context.Entry(planning).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Plannings.AnyAsync(p => p.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/plannings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlanning(int id)
        {
            var planning = await _context.Plannings.FindAsync(id);
            
            if (planning == null)
                return NotFound();

            _context.Plannings.Remove(planning);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}