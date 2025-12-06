using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models
{
    public class Table
    {
        [Key]
        public int NumeroTable { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Statut { get; set; }
        
        [MaxLength(50)]
        public string Emplacement { get; set; }
        
        // Foreign Key
        public int? IdServeur { get; set; }
        
        // Navigation Properties
        [ForeignKey("IdServeur")]
        public Serveur Serveur { get; set; }
        
        public ICollection<Reservation> Reservations { get; set; }
    }
}
