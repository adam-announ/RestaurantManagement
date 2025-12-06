using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.Models
{
    public class Commande
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime DateHeure { get; set; } = DateTime.Now;
        
        [MaxLength(20)]
        public string Statut { get; set; } = "En cours"; // En cours, Prête, Servie, Payée
        
        public double Total { get; set; }
        
        // Relations
        public int? ClientId { get; set; }
        [ForeignKey("ClientId")]
        public Client? Client { get; set; }
        
        public int? ServeurId { get; set; }
        [ForeignKey("ServeurId")]
        public Serveur? Serveur { get; set; }
        
        public int? CuisinierId { get; set; }
        [ForeignKey("CuisinierId")]
        public Cuisinier? Cuisinier { get; set; }
        
        public int? TableId { get; set; }
        [ForeignKey("TableId")]
        public Table? Table { get; set; }
        
        public ICollection<LigneCommande> LigneCommandes { get; set; } = new List<LigneCommande>();
        
        public Facture? Facture { get; set; }
    }
}