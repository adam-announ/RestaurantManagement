using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime Date { get; set; }
        
        public TimeSpan Heure { get; set; }
        
        public int NbPersonnes { get; set; }
        
        [MaxLength(20)]
        public string Statut { get; set; } = "En attente"; // En attente, Confirmée, Annulée
        
        // Relations
        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public Client? Client { get; set; }
        
        public int TableId { get; set; }
        [ForeignKey("TableId")]
        public Table? Table { get; set; }
    }
}