using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models
{
    public enum StatutReservation
    {
        CONFIRMEE,
        EN_ATTENTE,
        TERMINEE
    }

    public class Reservation
    {
        [Key]
        public int IdReservation { get; set; }
        
        [Required]
        public DateTime DateReservation { get; set; }
        
        [Required]
        public int NombrePersonnes { get; set; }
        
        [Required]
        public StatutReservation Statut { get; set; }
        
        // Foreign Keys
        public int IdClient { get; set; }
        public int IdTable { get; set; }
        
        // Navigation Properties
        [ForeignKey("IdClient")]
        public Client Client { get; set; }
        
        [ForeignKey("IdTable")]
        public Table Table { get; set; }
    }
}
