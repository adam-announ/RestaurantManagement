using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.Data;
using RestaurantAPI.Models;
using RestaurantAPI.Models.Auth;

namespace RestaurantAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly RestaurantContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(RestaurantContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponse?> Login(LoginRequest request)
        {
            var utilisateur = await _context.Utilisateurs
                .Include(u => u.Personne)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (utilisateur == null || !VerifyPassword(request.Password, utilisateur.PasswordHash))
                return null;

            var token = GenerateToken(utilisateur.Id, utilisateur.Email, utilisateur.Role);

            return new AuthResponse
            {
                Id = utilisateur.Id,
                Nom = utilisateur.Personne?.Nom ?? "",
                Prenom = utilisateur.Personne?.Prenom ?? "",
                Email = utilisateur.Email,
                Role = utilisateur.Role,
                Token = token
            };
        }

        public async Task<AuthResponse?> Register(RegisterRequest request)
        {
            // Vérifier si l'email existe déjà
            if (await _context.Utilisateurs.AnyAsync(u => u.Email == request.Email))
                return null;

            // Créer la personne selon le rôle
            Personne personne;
            switch (request.Role)
            {
                case "Serveur":
                    personne = new Serveur
                    {
                        Nom = request.Nom,
                        Prenom = request.Prenom,
                        Telephone = request.Telephone,
                        DateEmbauche = DateTime.UtcNow,
                        Poste = "Serveur"
                    };
                    _context.Serveurs.Add((Serveur)personne);
                    break;
                case "Cuisinier":
                    personne = new Cuisinier
                    {
                        Nom = request.Nom,
                        Prenom = request.Prenom,
                        Telephone = request.Telephone,
                        DateEmbauche = DateTime.UtcNow,
                        Poste = "Cuisinier"
                    };
                    _context.Cuisiniers.Add((Cuisinier)personne);
                    break;
                case "Manager":
                    personne = new Manager
                    {
                        Nom = request.Nom,
                        Prenom = request.Prenom,
                        Telephone = request.Telephone,
                        DateEmbauche = DateTime.UtcNow,
                        Poste = "Manager"
                    };
                    _context.Managers.Add((Manager)personne);
                    break;
                default: // Client
                    personne = new Client
                    {
                        Nom = request.Nom,
                        Prenom = request.Prenom,
                        Telephone = request.Telephone,
                        Email = request.Email
                    };
                    _context.Clients.Add((Client)personne);
                    break;
            }

            await _context.SaveChangesAsync();

            // Créer l'utilisateur
            var utilisateur = new Utilisateur
            {
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                Role = request.Role,
                PersonneId = personne.Id
            };

            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();

            var token = GenerateToken(utilisateur.Id, utilisateur.Email, utilisateur.Role);

            return new AuthResponse
            {
                Id = utilisateur.Id,
                Nom = personne.Nom,
                Prenom = personne.Prenom,
                Email = utilisateur.Email,
                Role = utilisateur.Role,
                Token = token
            };
        }

        public string GenerateToken(int userId, string email, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? "SuperSecretKeyForRestaurantApp2024!"));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private static bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }
}
