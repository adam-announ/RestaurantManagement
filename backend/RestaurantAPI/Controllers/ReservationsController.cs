using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.Models;

namespace RestaurantAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly RestaurantContext _context;

        public ReservationsController(RestaurantContext context)
        {
            _context = context;
        }

        // GET: api/reservations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            return await _context.Reservations
                .Include(r => r.Client)
                .Include(r => r.Table)
                .ToListAsync();
        }

        // GET: api/reservations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Client)
                .Include(r => r.Table)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return NotFound();

            return reservation;
        }

        // GET: api/reservations/date/2024-01-15
        [HttpGet("date/{date}")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservationsByDate(DateTime date)
        {
            return await _context.Reservations
                .Where(r => r.Date.Date == date.Date)
                .Include(r => r.Client)
                .Include(r => r.Table)
                .ToListAsync();
        }

        // POST: api/reservations
        [HttpPost]
        public async Task<ActionResult<Reservation>> CreateReservation(Reservation reservation)
        {
            // Vérifier si la table est disponible
            var tableOccupee = await _context.Reservations
                .AnyAsync(r => r.TableId == reservation.TableId 
                    && r.Date.Date == reservation.Date.Date
                    && r.Heure == reservation.Heure
                    && r.Statut != "Annulée");

            if (tableOccupee)
                return BadRequest("Cette table est déjà réservée pour ce créneau");

            // Vérifier la capacité de la table
            var table = await _context.Tables.FindAsync(reservation.TableId);
            if (table == null)
                return NotFound("Table non trouvée");

            if (table.Capacite < reservation.NbPersonnes)
                return BadRequest($"La table a une capacité de {table.Capacite} personnes");

            reservation.Statut = "En attente";
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
        }

        // PUT: api/reservations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReservation(int id, Reservation reservation)
        {
            if (id != reservation.Id)
                return BadRequest();

            _context.Entry(reservation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Reservations.AnyAsync(r => r.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // PATCH: api/reservations/5/confirmer
        [HttpPatch("{id}/confirmer")]
        public async Task<IActionResult> ConfirmerReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            
            if (reservation == null)
                return NotFound();

            reservation.Statut = "Confirmée";
            await _context.SaveChangesAsync();

            return Ok(reservation);
        }

        // PATCH: api/reservations/5/annuler
        [HttpPatch("{id}/annuler")]
        public async Task<IActionResult> AnnulerReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            
            if (reservation == null)
                return NotFound();

            reservation.Statut = "Annulée";
            await _context.SaveChangesAsync();

            return Ok(reservation);
        }

        // DELETE: api/reservations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            
            if (reservation == null)
                return NotFound();

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}