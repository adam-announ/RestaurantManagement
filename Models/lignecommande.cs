using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models
{
    public class LigneCommande
    {
        [Key]
        public int IdLigneCommande { get; set; }
        
        [Required]
        public int Quantite { get; set; }
        
        [Required]
        public double PrixUnitaire { get; set; }
        
        public double SousTotal { get; set; }
        
        // Foreign Keys
        public int IdCommande { get; set; }
        public int IdPlat { get; set; }
        
        // Navigation Properties
        [ForeignKey("IdCommande")]
        public Commande Commande { get; set; }
        
        [ForeignKey("IdPlat")]
        public Plat Plat { get; set; }
    }
}
