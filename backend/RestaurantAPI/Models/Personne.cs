using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Models
{
    public class Personne
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Nom { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Prenom { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? Telephone { get; set; }
    }
}