using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.Models;

namespace RestaurantAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommandesController : ControllerBase
    {
        private readonly RestaurantContext _context;

        public CommandesController(RestaurantContext context)
        {
            _context = context;
        }

        // GET: api/commandes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Commande>>> GetCommandes()
        {
            return await _context.Commandes
                .Include(c => c.Client)
                .Include(c => c.Serveur)
                .Include(c => c.Table)
                .Include(c => c.LigneCommandes)
                    .ThenInclude(lc => lc.Plat)
                .ToListAsync();
        }

        // GET: api/commandes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Commande>> GetCommande(int id)
        {
            var commande = await _context.Commandes
                .Include(c => c.Client)
                .Include(c => c.Serveur)
                .Include(c => c.Cuisinier)
                .Include(c => c.Table)
                .Include(c => c.LigneCommandes)
                    .ThenInclude(lc => lc.Plat)
                .Include(c => c.Facture)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (commande == null)
                return NotFound();

            return commande;
        }

        // GET: api/commandes/encours
        [HttpGet("encours")]
        public async Task<ActionResult<IEnumerable<Commande>>> GetCommandesEnCours()
        {
            return await _context.Commandes
                .Where(c => c.Statut == "En cours" || c.Statut == "Prête")
                .Include(c => c.Table)
                .Include(c => c.LigneCommandes)
                    .ThenInclude(lc => lc.Plat)
                .ToListAsync();
        }

        // GET: api/commandes/table/5
        [HttpGet("table/{tableId}")]
        public async Task<ActionResult<IEnumerable<Commande>>> GetCommandesByTable(int tableId)
        {
            return await _context.Commandes
                .Where(c => c.TableId == tableId)
                .Include(c => c.LigneCommandes)
                    .ThenInclude(lc => lc.Plat)
                .ToListAsync();
        }

        // POST: api/commandes
        [HttpPost]
        public async Task<ActionResult<Commande>> CreateCommande(Commande commande)
        {
            commande.DateHeure = DateTime.UtcNow;
            commande.Statut = "En cours";
            commande.Total = 0;

            _context.Commandes.Add(commande);
            await _context.SaveChangesAsync();

            // Mettre la table en statut Occupée
            if (commande.TableId.HasValue)
            {
                var table = await _context.Tables.FindAsync(commande.TableId);
                if (table != null)
                {
                    table.Statut = "Occupée";
                    await _context.SaveChangesAsync();
                }
            }

            return CreatedAtAction(nameof(GetCommande), new { id = commande.Id }, commande);
        }

        // POST: api/commandes/5/plats
        [HttpPost("{id}/plats")]
        public async Task<ActionResult<Commande>> AjouterPlat(int id, [FromBody] LigneCommandeDto dto)
        {
            var commande = await _context.Commandes
                .Include(c => c.LigneCommandes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (commande == null)
                return NotFound("Commande non trouvée");

            var plat = await _context.Plats.FindAsync(dto.PlatId);
            if (plat == null)
                return NotFound("Plat non trouvé");

            if (!plat.Disponible)
                return BadRequest("Ce plat n'est pas disponible");

            var ligneCommande = new LigneCommande
            {
                CommandeId = id,
                PlatId = dto.PlatId,
                Quantite = dto.Quantite,
                PrixUnitaire = plat.Prix
            };

            _context.LigneCommandes.Add(ligneCommande);
            
            // Recalculer le total
            commande.Total += plat.Prix * dto.Quantite;
            
            await _context.SaveChangesAsync();

            return Ok(commande);
        }

        // DELETE: api/commandes/5/plats/3
        [HttpDelete("{id}/plats/{ligneId}")]
        public async Task<IActionResult> SupprimerPlat(int id, int ligneId)
        {
            var commande = await _context.Commandes.FindAsync(id);
            if (commande == null)
                return NotFound("Commande non trouvée");

            var ligne = await _context.LigneCommandes.FindAsync(ligneId);
            if (ligne == null || ligne.CommandeId != id)
                return NotFound("Ligne de commande non trouvée");

            // Recalculer le total
            commande.Total -= ligne.PrixUnitaire * ligne.Quantite;

            _context.LigneCommandes.Remove(ligne);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/commandes/5/statut
        [HttpPatch("{id}/statut")]
        public async Task<IActionResult> UpdateStatut(int id, [FromBody] string statut)
        {
            var commande = await _context.Commandes.FindAsync(id);
            
            if (commande == null)
                return NotFound();

            commande.Statut = statut;
            await _context.SaveChangesAsync();

            return Ok(commande);
        }

        // PATCH: api/commandes/5/preparer
        [HttpPatch("{id}/preparer")]
        public async Task<IActionResult> PreparerCommande(int id, [FromBody] int cuisinierId)
        {
            var commande = await _context.Commandes.FindAsync(id);
            
            if (commande == null)
                return NotFound();

            commande.CuisinierId = cuisinierId;
            commande.Statut = "En préparation";
            await _context.SaveChangesAsync();

            return Ok(commande);
        }

        // PATCH: api/commandes/5/prete
        [HttpPatch("{id}/prete")]
        public async Task<IActionResult> CommandePrete(int id)
        {
            var commande = await _context.Commandes.FindAsync(id);
            
            if (commande == null)
                return NotFound();

            commande.Statut = "Prête";
            await _context.SaveChangesAsync();

            return Ok(commande);
        }

        // PATCH: api/commandes/5/servir
        [HttpPatch("{id}/servir")]
        public async Task<IActionResult> ServirCommande(int id)
        {
            var commande = await _context.Commandes.FindAsync(id);
            
            if (commande == null)
                return NotFound();

            commande.Statut = "Servie";
            await _context.SaveChangesAsync();

            return Ok(commande);
        }

        // DELETE: api/commandes/5
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
    }

    // DTO pour ajouter un plat
    public class LigneCommandeDto
    {
        public int PlatId { get; set; }
        public int Quantite { get; set; }
    }
}