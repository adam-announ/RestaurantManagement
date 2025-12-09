using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Models.Auth
{
    public class RegisterRequest
    {
        [Required]
        public string Nom { get; set; } = string.Empty;

        [Required]
        public string Prenom { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        public string? Telephone { get; set; }

        [Required]
        public string Role { get; set; } = "Client"; // Client, Serveur, Cuisinier, Manager
    }
}
