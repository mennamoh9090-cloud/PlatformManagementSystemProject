namespace PlatformManagementSystem.MVC.ViewModels.Admin
{
    public class ManageUserRolesVM
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public List<string> UserRoles { get; set; } = new();
        public List<AdminRoleVM> AvailableRoles { get; set; } = new();
    }
}
