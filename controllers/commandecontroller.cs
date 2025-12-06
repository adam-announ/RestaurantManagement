using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;

namespace RestaurantManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommandesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Commande>>> GetCommandes()
        {
            return await _context.Commandes
                .Include(c => c.Client)
                .Include(c => c.LignesCommande)
                .ThenInclude(lc => lc.Plat)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Commande>> GetCommande(int id)
        {
            var commande = await _context.Commandes
                .Include(c => c.Client)
                .Include(c => c.LignesCommande)
                .ThenInclude(lc => lc.Plat)
                .Include(c => c.Paiement)
                .FirstOrDefaultAsync(c => c.IdCommande == id);

            if (commande == null)
                return NotFound();

            return commande;
        }

        [HttpPost]
        public async Task<ActionResult<Commande>> PostCommande(Commande commande)
        {
            commande.DateCommande = DateTime.Now;
            commande.HeureCommande = DateTime.Now.TimeOfDay;
            commande.Statut = StatutCommande.EN_COURS;

            // Calculer montant total
            double total = 0;
            foreach (var ligne in commande.LignesCommande)
            {
                var plat = await _context.Plats.FindAsync(ligne.IdPlat);
                if (plat != null)
                {
                    ligne.PrixUnitaire = plat.Prix;
                    ligne.SousTotal = ligne.Quantite * ligne.PrixUnitaire;
                    total += ligne.SousTotal;
                }
            }
            commande.MontantTotal = total;

            _context.Commandes.Add(commande);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCommande), new { id = commande.IdCommande }, commande);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCommande(int id, Commande commande)
        {
            if (id != commande.IdCommande)
                return BadRequest();

            _context.Entry(commande).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommande(int id)
        {
            var commande = await _context.Commandes.FindAsync(id);
            if (commande == null)
                return NotFound();

            _context.Commandes.Remove(commande);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/CalculerTotal")]
        public async Task<ActionResult<double>> CalculerTotal(int id)
        {
            var commande = await _context.Commandes
                .Include(c => c.LignesCommande)
                .FirstOrDefaultAsync(c => c.IdCommande == id);

            if (commande == null)
                return NotFound();

            double total = commande.LignesCommande.Sum(lc => lc.SousTotal);
            commande.MontantTotal = total;
            await _context.SaveChangesAsync();

            return Ok(total);
        }

        [HttpPost("{id}/Valider")]
        public async Task<IActionResult> ValiderCommande(int id)
        {
            var commande = await _context.Commandes.FindAsync(id);
            if (commande == null)
                return NotFound();

            commande.Statut = StatutCommande.EN_PREPARATION;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Commande validée" });
        }
    }
}
