using Microsoft.AspNetCore.Identity;
using PlatformManagementSystem.Application.DTOs.Auth;
using PlatformManagementSystem.Application.Interfaces.Services;
using PlatformManagementSystem.Domain.Entities;

namespace PlatformManagementSystem.Application.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager) : IAuthService
    {
        public async Task<LoginResponseDto?> LoginAsync(LoginDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return null;

            var validPassword = await userManager.CheckPasswordAsync(user, model.Password);

            if (!validPassword)
                return null;

            var roles = await userManager.GetRolesAsync(user);

            return new LoginResponseDto
            {
                Email = user.Email ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                Role = roles.FirstOrDefault() ?? string.Empty,
                UserId = user.Id,
                Token = "" 
            };
        }
    }
}
