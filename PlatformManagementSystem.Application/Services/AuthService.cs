using Microsoft.AspNetCore.Identity;
using PlatformManagementSystem.Application.DTOs.Auth;
using PlatformManagementSystem.Application.Interfaces.Services;
using PlatformManagementSystem.Domain.Entities;

namespace PlatformManagementSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return null;

            var validPassword = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!validPassword)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new LoginResponseDto
            {
                Email = user.Email,
                FullName = user.FullName,
                Role = roles.FirstOrDefault(),
                UserId = user.Id,
                Token = "" 
            };
        }
    }
}
