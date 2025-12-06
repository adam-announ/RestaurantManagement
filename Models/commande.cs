using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models
{
    public enum StatutCommande
    {
        EN_COURS,
        EN_PREPARATION,
        PRETE,
        ANNULEE
    }

    public class Commande
    {
        [Key]
        public int IdCommande { get; set; }
        
        [Required]
        public DateTime DateCommande { get; set; }
        
        public TimeSpan HeureCommande { get; set; }
        
        [Required]
        public double MontantTotal { get; set; }
        
        [Required]
        public StatutCommande Statut { get; set; }
        
        // Foreign Key
        public int IdClient { get; set; }
        
        // Navigation Properties
        [ForeignKey("IdClient")]
        public Client Client { get; set; }
        
        public ICollection<LigneCommande> LignesCommande { get; set; }
        public Paiement Paiement { get; set; }
    }
}
