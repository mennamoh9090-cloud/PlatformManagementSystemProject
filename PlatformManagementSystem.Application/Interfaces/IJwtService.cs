using PlatformManagementSystem.Domain.Entities;

namespace PlatformManagementSystem.Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(ApplicationUser user, string role);
}
