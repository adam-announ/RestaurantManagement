using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Models
{
    public class Plat
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Nom { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public double Prix { get; set; }
        
        [MaxLength(50)]
        public string? Categorie { get; set; }
        
        public bool Disponible { get; set; } = true;
        
        // Relations
        public ICollection<LigneCommande> LigneCommandes { get; set; } = new List<LigneCommande>();
        public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    }
}