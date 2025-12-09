using RestaurantAPI.Models.Auth;

namespace RestaurantAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> Login(LoginRequest request);
        Task<AuthResponse?> Register(RegisterRequest request);
        string GenerateToken(int userId, string email, string role);
    }
}
