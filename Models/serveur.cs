using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Models
{
    public class Serveur
    {
        [Key]
        public int IdServeur { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Secteur { get; set; }
        
        public double Pourboires { get; set; }
        
        // Relations
        public ICollection<Table> Tables { get; set; }
    }
}
