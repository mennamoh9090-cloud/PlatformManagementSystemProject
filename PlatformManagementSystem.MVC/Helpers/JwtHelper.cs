using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PlatformManagementSystem.MVC.Helpers
{
    public static class JwtHelper
    {
        public static string? GetRoleFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var roleClaim = jwtToken.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Role);

            return roleClaim?.Value;
        }

        public static string? GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userIdClaim = jwtToken.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            return userIdClaim?.Value;
        }
    }
}

