using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;

namespace RestaurantAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatistiquesController : ControllerBase
    {
        private readonly RestaurantContext _context;

        public StatistiquesController(RestaurantContext context)
        {
            _context = context;
        }

        // GET: api/statistiques/dashboard
        [HttpGet("dashboard")]
        public async Task<ActionResult> GetDashboardStats()
        {
            var today = DateTime.UtcNow.Date;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            var stats = new
            {
                // Aujourd'hui
                VentesAujourdhui = await _context.Factures
                    .Where(f => f.Date.Date == today && f.Statut == "Payée")
                    .SumAsync(f => f.MontantTotal),
                
                CommandesAujourdhui = await _context.Commandes
                    .CountAsync(c => c.DateHeure.Date == today),
                
                ReservationsAujourdhui = await _context.Reservations
                    .CountAsync(r => r.Date.Date == today),

                // Ce mois
                VentesMois = await _context.Factures
                    .Where(f => f.Date >= startOfMonth && f.Statut == "Payée")
                    .SumAsync(f => f.MontantTotal),
                
                CommandesMois = await _context.Commandes
                    .CountAsync(c => c.DateHeure >= startOfMonth),

                // Général
                TotalClients = await _context.Clients.CountAsync(),
                TotalEmployes = await _context.Employes.CountAsync(),
                TablesDisponibles = await _context.Tables.CountAsync(t => t.Statut == "Disponible"),
                TotalTables = await _context.Tables.CountAsync(),
                StocksEnAlerte = await _context.Stocks
                    .Include(s => s.Ingredient)
                    .CountAsync(s => s.Ingredient != null && s.Quantite <= s.Ingredient.SeuilAlerte)
            };

            return Ok(stats);
        }

        // GET: api/statistiques/ventes/jour?date=2024-01-15
        [HttpGet("ventes/jour")]
        public async Task<ActionResult> GetVentesParJour([FromQuery] DateTime? date)
        {
            var targetDate = date?.Date ?? DateTime.UtcNow.Date;

            var ventes = await _context.Factures
                .Where(f => f.Date.Date == targetDate && f.Statut == "Payée")
                .Include(f => f.Commande)
                    .ThenInclude(c => c.LigneCommandes)
                        .ThenInclude(lc => lc.Plat)
                .ToListAsync();

            var result = new
            {
                Date = targetDate,
                TotalVentes = ventes.Sum(f => f.MontantTotal),
                NombreFactures = ventes.Count,
                Factures = ventes.Select(f => new
                {
                    f.Id,
                    f.MontantTotal,
                    f.ModePaiement,
                    Heure = f.Date.ToString("HH:mm")
                })
            };

            return Ok(result);
        }

        // GET: api/statistiques/ventes/mois?annee=2024&mois=1
        [HttpGet("ventes/mois")]
        public async Task<ActionResult> GetVentesParMois([FromQuery] int? annee, [FromQuery] int? mois)
        {
            var year = annee ?? DateTime.UtcNow.Year;
            var month = mois ?? DateTime.UtcNow.Month;
            var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var endDate = startDate.AddMonths(1);

            var ventesParJour = await _context.Factures
                .Where(f => f.Date >= startDate && f.Date < endDate && f.Statut == "Payée")
                .GroupBy(f => f.Date.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Total = g.Sum(f => f.MontantTotal),
                    Nombre = g.Count()
                })
                .OrderBy(v => v.Date)
                .ToListAsync();

            var result = new
            {
                Annee = year,
                Mois = month,
                TotalMois = ventesParJour.Sum(v => v.Total),
                NombreFactures = ventesParJour.Sum(v => v.Nombre),
                VentesParJour = ventesParJour
            };

            return Ok(result);
        }

        // GET: api/statistiques/ventes/annee?annee=2024
        [HttpGet("ventes/annee")]
        public async Task<ActionResult> GetVentesParAnnee([FromQuery] int? annee)
        {
            var year = annee ?? DateTime.UtcNow.Year;
            var startDate = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var endDate = startDate.AddYears(1);

            var ventesParMois = await _context.Factures
                .Where(f => f.Date >= startDate && f.Date < endDate && f.Statut == "Payée")
                .GroupBy(f => f.Date.Month)
                .Select(g => new
                {
                    Mois = g.Key,
                    Total = g.Sum(f => f.MontantTotal),
                    Nombre = g.Count()
                })
                .OrderBy(v => v.Mois)
                .ToListAsync();

            var result = new
            {
                Annee = year,
                TotalAnnee = ventesParMois.Sum(v => v.Total),
                NombreFactures = ventesParMois.Sum(v => v.Nombre),
                VentesParMois = ventesParMois
            };

            return Ok(result);
        }

        // GET: api/statistiques/plats-populaires
        [HttpGet("plats-populaires")]
        public async Task<ActionResult> GetPlatsPopulaires([FromQuery] int limit = 10)
        {
            var platsPopulaires = await _context.LigneCommandes
                .Include(lc => lc.Plat)
                .GroupBy(lc => new { lc.PlatId, lc.Plat!.Nom, lc.Plat.Prix, lc.Plat.Categorie })
                .Select(g => new
                {
                    PlatId = g.Key.PlatId,
                    Nom = g.Key.Nom,
                    Prix = g.Key.Prix,
                    Categorie = g.Key.Categorie,
                    QuantiteVendue = g.Sum(lc => lc.Quantite),
                    ChiffreAffaires = g.Sum(lc => lc.Quantite * lc.PrixUnitaire)
                })
                .OrderByDescending(p => p.QuantiteVendue)
                .Take(limit)
                .ToListAsync();

            return Ok(platsPopulaires);
        }

        // GET: api/statistiques/revenus-par-categorie
        [HttpGet("revenus-par-categorie")]
        public async Task<ActionResult> GetRevenusParCategorie()
        {
            var revenusParCategorie = await _context.LigneCommandes
                .Include(lc => lc.Plat)
                .Where(lc => lc.Plat != null)
                .GroupBy(lc => lc.Plat!.Categorie ?? "Non catégorisé")
                .Select(g => new
                {
                    Categorie = g.Key,
                    ChiffreAffaires = g.Sum(lc => lc.Quantite * lc.PrixUnitaire),
                    NombrePlatsVendus = g.Sum(lc => lc.Quantite)
                })
                .OrderByDescending(c => c.ChiffreAffaires)
                .ToListAsync();

            return Ok(revenusParCategorie);
        }

        // GET: api/statistiques/heures-pointe
        [HttpGet("heures-pointe")]
        public async Task<ActionResult> GetHeuresPointe()
        {
            var commandesParHeure = await _context.Commandes
                .GroupBy(c => c.DateHeure.Hour)
                .Select(g => new
                {
                    Heure = g.Key,
                    NombreCommandes = g.Count()
                })
                .OrderBy(h => h.Heure)
                .ToListAsync();

            return Ok(commandesParHeure);
        }

        // GET: api/statistiques/performance-serveurs
        [HttpGet("performance-serveurs")]
        public async Task<ActionResult> GetPerformanceServeurs()
        {
            var performanceServeurs = await _context.Commandes
                .Where(c => c.ServeurId != null)
                .Include(c => c.Serveur)
                .GroupBy(c => new { c.ServeurId, c.Serveur!.Nom, c.Serveur.Prenom })
                .Select(g => new
                {
                    ServeurId = g.Key.ServeurId,
                    Nom = g.Key.Nom + " " + g.Key.Prenom,
                    NombreCommandes = g.Count(),
                    ChiffreAffaires = g.Sum(c => c.Total)
                })
                .OrderByDescending(s => s.ChiffreAffaires)
                .ToListAsync();

            return Ok(performanceServeurs);
        }
    }
}
