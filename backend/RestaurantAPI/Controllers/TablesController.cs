using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.Models;

namespace RestaurantAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TablesController : ControllerBase
    {
        private readonly RestaurantContext _context;

        public TablesController(RestaurantContext context)
        {
            _context = context;
        }

        // GET: api/tables
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Table>>> GetTables()
        {
            return await _context.Tables.ToListAsync();
        }

        // GET: api/tables/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Table>> GetTable(int id)
        {
            var table = await _context.Tables.FindAsync(id);

            if (table == null)
                return NotFound();

            return table;
        }

        // GET: api/tables/disponibles
        [HttpGet("disponibles")]
        public async Task<ActionResult<IEnumerable<Table>>> GetTablesDisponibles()
        {
            return await _context.Tables
                .Where(t => t.Statut == "Disponible")
                .ToListAsync();
        }

        // POST: api/tables
        [HttpPost]
        public async Task<ActionResult<Table>> CreateTable(Table table)
        {
            _context.Tables.Add(table);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTable), new { id = table.Id }, table);
        }

        // PUT: api/tables/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTable(int id, Table table)
        {
            if (id != table.Id)
                return BadRequest();

            _context.Entry(table).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Tables.AnyAsync(t => t.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // PATCH: api/tables/5/statut
        [HttpPatch("{id}/statut")]
        public async Task<IActionResult> UpdateStatut(int id, [FromBody] string statut)
        {
            var table = await _context.Tables.FindAsync(id);
            
            if (table == null)
                return NotFound();

            table.Statut = statut;
            await _context.SaveChangesAsync();

            return Ok(table);
        }

        // DELETE: api/tables/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTable(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            
            if (table == null)
                return NotFound();

            _context.Tables.Remove(table);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}