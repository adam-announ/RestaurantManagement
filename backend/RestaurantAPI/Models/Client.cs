using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Models
{
    public class Client : Personne
    {
        [EmailAddress]
        public string? Email { get; set; }
        
        // Relations
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Commande> Commandes { get; set; } = new List<Commande>();
    }
}