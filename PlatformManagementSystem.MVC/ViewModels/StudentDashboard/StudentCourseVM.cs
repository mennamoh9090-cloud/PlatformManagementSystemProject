namespace PlatformManagementSystem.MVC.ViewModels.StudentDashboard
{
    public class StudentCourseVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public int ProgressPercentage { get; set; }
        public bool IsLive { get; set; }
        public int? LiveSessionId { get; set; }
    }
}

