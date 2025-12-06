using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;

namespace RestaurantManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReservationsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            return await _context.Reservations
                .Include(r => r.Client)
                .Include(r => r.Table)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Client)
                .Include(r => r.Table)
                .FirstOrDefaultAsync(r => r.IdReservation == id);

            if (reservation == null)
                return NotFound();

            return reservation;
        }

        [HttpPost]
        public async Task<ActionResult<Reservation>> PostReservation(Reservation reservation)
        {
            // Vérifier disponibilité de la table
            var tableDisponible = await _context.Tables
                .Where(t => t.NumeroTable == reservation.IdTable && t.Statut == "LIBRE")
                .AnyAsync();

            if (!tableDisponible)
                return BadRequest("Table non disponible");

            reservation.Statut = StatutReservation.EN_ATTENTE;
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReservation), new { id = reservation.IdReservation }, reservation);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(int id, Reservation reservation)
        {
            if (id != reservation.IdReservation)
                return BadRequest();

            _context.Entry(reservation).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

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

        [HttpPost("{id}/Confirmer")]
        public async Task<IActionResult> ConfirmerReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return NotFound();

            reservation.Statut = StatutReservation.CONFIRMEE;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Réservation confirmée" });
        }
    }
}
