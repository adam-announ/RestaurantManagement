using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Models
{
    public class Utilisateur
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Client"; // Client, Serveur, Cuisinier, Manager

        public int? PersonneId { get; set; }
        public Personne? Personne { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    }
}
