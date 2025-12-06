using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Models
{
    public class Cuisinier : Employe
    {
        [MaxLength(100)]
        public string? Specialite { get; set; }
        
        // Relations
        public ICollection<Commande> CommandesPreparees { get; set; } = new List<Commande>();
    }
}