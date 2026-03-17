using System.Security.Claims;

namespace PlatformManagementSystem.MVC.ViewModels.Auth
{
    public class AuthResponse
    {
        public string Token { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";

        public string Role { get; set; } = "";
    }
}
