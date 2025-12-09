using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.Models;

namespace RestaurantAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StocksController : ControllerBase
    {
        private readonly RestaurantContext _context;

        public StocksController(RestaurantContext context)
        {
            _context = context;
        }

        // GET: api/stocks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stock>>> GetStocks()
        {
            return await _context.Stocks
                .Include(s => s.Ingredient)
                .ToListAsync();
        }

        // GET: api/stocks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Stock>> GetStock(int id)
        {
            var stock = await _context.Stocks
                .Include(s => s.Ingredient)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (stock == null)
                return NotFound();

            return stock;
        }

        // GET: api/stocks/ingredient/5
        [HttpGet("ingredient/{ingredientId}")]
        public async Task<ActionResult<Stock>> GetStockByIngredient(int ingredientId)
        {
            var stock = await _context.Stocks
                .Include(s => s.Ingredient)
                .FirstOrDefaultAsync(s => s.IngredientId == ingredientId);

            if (stock == null)
                return NotFound();

            return stock;
        }

        // GET: api/stocks/faibles
        [HttpGet("faibles")]
        public async Task<ActionResult<IEnumerable<Stock>>> GetStocksFaibles()
        {
            return await _context.Stocks
                .Include(s => s.Ingredient)
                .Where(s => s.Ingredient != null && s.Quantite <= s.Ingredient.SeuilAlerte)
                .ToListAsync();
        }

        // PATCH: api/stocks/5/ajouter
        [HttpPatch("{id}/ajouter")]
        public async Task<IActionResult> AjouterStock(int id, [FromBody] int quantite)
        {
            var stock = await _context.Stocks.FindAsync(id);
            
            if (stock == null)
                return NotFound();

            stock.Quantite += quantite;
            stock.DateMAJ = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(stock);
        }

        // PATCH: api/stocks/5/retirer
        [HttpPatch("{id}/retirer")]
        public async Task<IActionResult> RetirerStock(int id, [FromBody] int quantite)
        {
            var stock = await _context.Stocks
                .Include(s => s.Ingredient)
                .FirstOrDefaultAsync(s => s.Id == id);
            
            if (stock == null)
                return NotFound();

            if (stock.Quantite < quantite)
                return BadRequest("Stock insuffisant");

            stock.Quantite -= quantite;
            stock.DateMAJ = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Vérifier alerte
            if (stock.Ingredient != null && stock.Quantite <= stock.Ingredient.SeuilAlerte)
            {
                return Ok(new { 
                    stock, 
                    alerte = $"Attention: stock de {stock.Ingredient.Nom} en dessous du seuil d'alerte!" 
                });
            }

            return Ok(stock);
        }

        // PUT: api/stocks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] int nouvelleQuantite)
        {
            var stock = await _context.Stocks.FindAsync(id);
            
            if (stock == null)
                return NotFound();

            stock.Quantite = nouvelleQuantite;
            stock.DateMAJ = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(stock);
        }
    }
}