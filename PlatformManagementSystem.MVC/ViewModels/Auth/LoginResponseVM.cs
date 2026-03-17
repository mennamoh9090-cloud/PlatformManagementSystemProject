namespace PlatformManagementSystem.MVC.ViewModels.Auth
{
    public class LoginResponseVM
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string UserId { get; set; }   
    }
}