using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }
        
        public int Quantite { get; set; }
        
        public DateTime DateMAJ { get; set; } = DateTime.Now;
        
        // Relations
        public int IngredientId { get; set; }
        [ForeignKey("IngredientId")]
        public Ingredient? Ingredient { get; set; }
    }
}