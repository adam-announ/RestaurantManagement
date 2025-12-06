using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.Models
{
    public class Facture
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime Date { get; set; } = DateTime.Now;
        
        public double MontantTotal { get; set; }
        
        [MaxLength(50)]
        public string? ModePaiement { get; set; } // Espèces, CB, Chèque
        
        [MaxLength(20)]
        public string Statut { get; set; } = "Non payée"; // Non payée, Payée
        
        // Relations
        public int CommandeId { get; set; }
        [ForeignKey("CommandeId")]
        public Commande? Commande { get; set; }
    }
}