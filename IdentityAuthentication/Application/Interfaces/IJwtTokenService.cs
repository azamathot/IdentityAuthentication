using System.Security.Claims;

namespace WebApplication5.Application.Interfaces
{
    public interface IJwtTokenService
    {
        Task<string> GenerateAccessTokenAsync(ApplicationUser user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        string ValidateToken(string token); // Проверка токена, возвращает UserId или null
        string GetSecurityStampFromToken(string token); // Извлекает SecurityStamp
    }
}
