using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;

namespace RestaurantManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Clients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            return await _context.Clients
                .Include(c => c.Reservations)
                .Include(c => c.Commandes)
                .ToListAsync();
        }

        // GET: api/Clients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _context.Clients
                .Include(c => c.Reservations)
                .Include(c => c.Commandes)
                .FirstOrDefaultAsync(c => c.IdClient == id);

            if (client == null)
                return NotFound();

            return client;
        }

        // POST: api/Clients
        [HttpPost]
        public async Task<ActionResult<Client>> PostClient(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClient), new { id = client.IdClient }, client);
        }

        // PUT: api/Clients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient(int id, Client client)
        {
            if (id != client.IdClient)
                return BadRequest();

            _context.Entry(client).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Clients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                return NotFound();

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Clients/5/Reservations
        [HttpGet("{id}/Reservations")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetClientReservations(int id)
        {
            var client = await _context.Clients
                .Include(c => c.Reservations)
                .ThenInclude(r => r.Table)
                .FirstOrDefaultAsync(c => c.IdClient == id);

            if (client == null)
                return NotFound();

            return Ok(client.Reservations);
        }

        // GET: api/Clients/5/Commandes
        [HttpGet("{id}/Commandes")]
        public async Task<ActionResult<IEnumerable<Commande>>> GetClientCommandes(int id)
        {
            var client = await _context.Clients
                .Include(c => c.Commandes)
                .ThenInclude(cmd => cmd.LignesCommande)
                .FirstOrDefaultAsync(c => c.IdClient == id);

            if (client == null)
                return NotFound();

            return Ok(client.Commandes);
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.IdClient == id);
        }
    }
}
