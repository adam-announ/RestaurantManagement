using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Models
{
    public class Plat
    {
        [Key]
        public int IdPlat { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Nom { get; set; }
        
        [MaxLength(500)]
        public string Description { get; set; }
        
        [Required]
        public double Prix { get; set; }
        
        [MaxLength(50)]
        public string Categorie { get; set; }
        
        public int? TempsPreparation { get; set; }
        
        public bool Disponible { get; set; }
        
        [MaxLength(255)]
        public string ImageUrl { get; set; }
        
        // Relations
        public ICollection<LigneCommande> LignesCommande { get; set; }
    }
}
