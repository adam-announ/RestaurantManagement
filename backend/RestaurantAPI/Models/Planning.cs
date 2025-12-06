using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.Models
{
    public class Planning
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime Date { get; set; }
        
        public TimeSpan HeureDebut { get; set; }
        
        public TimeSpan HeureFin { get; set; }
        
        // Relations
        public int EmployeId { get; set; }
        [ForeignKey("EmployeId")]
        public Employe? Employe { get; set; }
    }
}