namespace PlatformManagementSystem.MVC.ViewModels.Admin
{
    public class AdminCourseVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public int Status { get; set; }
        public string StatusName { get; set; } = "Pending";
        public int StudentsCount { get; set; }
    }
}
