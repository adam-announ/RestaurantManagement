using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Models
{
    public class Table
    {
        [Key]
        public int Id { get; set; }
        
        public int Numero { get; set; }
        
        public int Capacite { get; set; }
        
        [MaxLength(20)]
        public string Statut { get; set; } = "Disponible"; // Disponible, Occupée, Réservée
        
        // Relations
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Commande> Commandes { get; set; } = new List<Commande>();
    }
}