using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Models
{
    public class Serveur : Employe
    {
        [MaxLength(50)]
        public string? Zone { get; set; }
        
        // Relations
        public ICollection<Commande> Commandes { get; set; } = new List<Commande>();
    }
}