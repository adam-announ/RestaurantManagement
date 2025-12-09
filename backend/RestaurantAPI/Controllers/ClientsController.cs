using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.Models;

namespace RestaurantAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly RestaurantContext _context;

        public ClientsController(RestaurantContext context)
        {
            _context = context;
        }

        // GET: api/clients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            return await _context.Clients.ToListAsync();
        }

        // GET: api/clients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _context.Clients
                .Include(c => c.Reservations)
                .Include(c => c.Commandes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
                return NotFound();

            return client;
        }

        // POST: api/clients
        [HttpPost]
        public async Task<ActionResult<Client>> CreateClient(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
        }

        // PUT: api/clients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, Client client)
        {
            if (id != client.Id)
                return BadRequest();

            _context.Entry(client).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Clients.AnyAsync(c => c.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/clients/5
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

        // GET: api/clients/5/reservations
        [HttpGet("{id}/reservations")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetClientReservations(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            
            if (client == null)
                return NotFound();

            var reservations = await _context.Reservations
                .Where(r => r.ClientId == id)
                .Include(r => r.Table)
                .ToListAsync();

            return reservations;
        }

        // GET: api/clients/5/commandes
        [HttpGet("{id}/commandes")]
        public async Task<ActionResult<IEnumerable<Commande>>> GetClientCommandes(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            
            if (client == null)
                return NotFound();

            var commandes = await _context.Commandes
                .Where(c => c.ClientId == id)
                .Include(c => c.LigneCommandes)
                    .ThenInclude(lc => lc.Plat)
                .ToListAsync();

            return commandes;
        }
    }
}