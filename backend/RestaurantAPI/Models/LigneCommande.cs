using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.Models
{
    public class LigneCommande
    {
        [Key]
        public int Id { get; set; }
        
        public int Quantite { get; set; }
        
        public double PrixUnitaire { get; set; }
        
        // Relations
        public int CommandeId { get; set; }
        [ForeignKey("CommandeId")]
        public Commande? Commande { get; set; }
        
        public int PlatId { get; set; }
        [ForeignKey("PlatId")]
        public Plat? Plat { get; set; }
    }
}