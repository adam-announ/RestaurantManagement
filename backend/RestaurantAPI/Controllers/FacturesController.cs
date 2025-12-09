using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.Models;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturesController : ControllerBase
    {
        private readonly RestaurantContext _context;
        private readonly PdfService _pdfService;

        public FacturesController(RestaurantContext context)
        {
            _context = context;
            _pdfService = new PdfService();
        }

        // GET: api/factures
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Facture>>> GetFactures()
        {
            return await _context.Factures
                .Include(f => f.Commande)
                    .ThenInclude(c => c.Client)
                .ToListAsync();
        }

        // GET: api/factures/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Facture>> GetFacture(int id)
        {
            var facture = await _context.Factures
                .Include(f => f.Commande)
                    .ThenInclude(c => c.Client)
                .Include(f => f.Commande)
                    .ThenInclude(c => c.LigneCommandes)
                        .ThenInclude(lc => lc.Plat)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (facture == null)
                return NotFound();

            return facture;
        }

        // GET: api/factures/5/pdf
        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> GetFacturePdf(int id)
        {
            var facture = await _context.Factures
                .Include(f => f.Commande)
                    .ThenInclude(c => c.Client)
                .Include(f => f.Commande)
                    .ThenInclude(c => c.Table)
                .Include(f => f.Commande)
                    .ThenInclude(c => c.LigneCommandes)
                        .ThenInclude(lc => lc.Plat)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (facture == null || facture.Commande == null)
                return NotFound();

            var pdfBytes = _pdfService.GenerateFacturePdf(facture, facture.Commande);

            return File(pdfBytes, "application/pdf", $"Facture_{id:D6}.pdf");
        }

        // POST: api/factures/commande/5
        [HttpPost("commande/{commandeId}")]
        public async Task<ActionResult<Facture>> GenererFacture(int commandeId)
        {
            var commande = await _context.Commandes
                .Include(c => c.Facture)
                .FirstOrDefaultAsync(c => c.Id == commandeId);

            if (commande == null)
                return NotFound("Commande non trouvée");

            if (commande.Facture != null)
                return BadRequest("Une facture existe déjà pour cette commande");

            var facture = new Facture
            {
                CommandeId = commandeId,
                Date = DateTime.UtcNow,
                MontantTotal = commande.Total,
                Statut = "Non payée"
            };

            _context.Factures.Add(facture);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFacture), new { id = facture.Id }, facture);
        }

        // PATCH: api/factures/5/payer
        [HttpPatch("{id}/payer")]
        public async Task<IActionResult> PayerFacture(int id, [FromBody] string modePaiement)
        {
            var facture = await _context.Factures
                .Include(f => f.Commande)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (facture == null)
                return NotFound();

            facture.ModePaiement = modePaiement;
            facture.Statut = "Payée";

            if (facture.Commande != null)
            {
                facture.Commande.Statut = "Payée";

                if (facture.Commande.TableId.HasValue)
                {
                    var table = await _context.Tables.FindAsync(facture.Commande.TableId);
                    if (table != null)
                    {
                        table.Statut = "Disponible";
                    }
                }
            }

            await _context.SaveChangesAsync();

            return Ok(facture);
        }

        // DELETE: api/factures/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFacture(int id)
        {
            var facture = await _context.Factures.FindAsync(id);

            if (facture == null)
                return NotFound();

            _context.Factures.Remove(facture);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
