using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Models
{
    public class Ingredient
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Nom { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? Unite { get; set; }
        
        public int SeuilAlerte { get; set; }
        
        // Relations
        public Stock? Stock { get; set; }
        public ICollection<Plat> Plats { get; set; } = new List<Plat>();
    }
}