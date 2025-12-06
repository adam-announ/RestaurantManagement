using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Models
{
    public class Client
    {
        [Key]
        public int IdClient { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Nom { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Prenom { get; set; }
        
        [MaxLength(20)]
        public string Telephone { get; set; }
        
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }
        
        // Relations
        public ICollection<Reservation> Reservations { get; set; }
        public ICollection<Commande> Commandes { get; set; }
    }
}
