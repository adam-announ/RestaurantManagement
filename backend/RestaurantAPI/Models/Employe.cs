using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Models
{
    public class Employe : Personne
    {
        public DateTime DateEmbauche { get; set; }
        
        public double Salaire { get; set; }
        
        [MaxLength(50)]
        public string? Poste { get; set; }
        
        // Relations
        public ICollection<Planning> Plannings { get; set; } = new List<Planning>();
    }
}