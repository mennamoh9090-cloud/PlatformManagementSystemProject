namespace PlatformManagementSystem.MVC.ViewModels.Admin
{
    public class AdminDashboardVM
    {
        public int TotalUsers { get; set; }
        public int TotalStudents { get; set; }
        public int TotalInstructors { get; set; }
        public int TotalCourses { get; set; }
        public int TotalEnrollments { get; set; }

        public List<AdminUserVM> LatestUsers { get; set; }
    }
}