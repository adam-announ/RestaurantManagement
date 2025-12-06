using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models
{
    public enum MethodePaiement
    {
        CARTE_BANCAIRE,
        CHEQUE,
        MOBILE
    }

    public class Paiement
    {
        [Key]
        public int IdPaiement { get; set; }
        
        [Required]
        public double Montant { get; set; }
        
        [Required]
        public DateTime DatePaiement { get; set; }
        
        [Required]
        public MethodePaiement MethodePaiement { get; set; }
        
        [MaxLength(50)]
        public string Statut { get; set; }
        
        // Foreign Key
        public int IdCommande { get; set; }
        
        // Navigation Property
        [ForeignKey("IdCommande")]
        public Commande Commande { get; set; }
    }
}
