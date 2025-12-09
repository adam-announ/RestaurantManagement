using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.Models;

namespace RestaurantAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly RestaurantContext _context;

        public IngredientsController(RestaurantContext context)
        {
            _context = context;
        }

        // GET: api/ingredients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetIngredients()
        {
            return await _context.Ingredients
                .Include(i => i.Stock)
                .ToListAsync();
        }

        // GET: api/ingredients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Ingredient>> GetIngredient(int id)
        {
            var ingredient = await _context.Ingredients
                .Include(i => i.Stock)
                .Include(i => i.Plats)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (ingredient == null)
                return NotFound();

            return ingredient;
        }

        // GET: api/ingredients/alertes
        [HttpGet("alertes")]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetIngredientsEnAlerte()
        {
            return await _context.Ingredients
                .Include(i => i.Stock)
                .Where(i => i.Stock != null && i.Stock.Quantite <= i.SeuilAlerte)
                .ToListAsync();
        }

        // POST: api/ingredients
        [HttpPost]
        public async Task<ActionResult<Ingredient>> CreateIngredient(Ingredient ingredient)
        {
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();

            // Créer le stock initial
            var stock = new Stock
            {
                IngredientId = ingredient.Id,
                Quantite = 0,
                DateMAJ = DateTime.UtcNow
            };
            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetIngredient), new { id = ingredient.Id }, ingredient);
        }

        // PUT: api/ingredients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIngredient(int id, Ingredient ingredient)
        {
            if (id != ingredient.Id)
                return BadRequest();

            _context.Entry(ingredient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Ingredients.AnyAsync(i => i.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/ingredients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            
            if (ingredient == null)
                return NotFound();

            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}