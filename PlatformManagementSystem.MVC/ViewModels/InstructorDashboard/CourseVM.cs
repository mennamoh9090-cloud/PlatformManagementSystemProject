namespace PlatformManagementSystem.MVC.ViewModels.InstructorDashboard
{
    public class CourseVM
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public int LessonsCount { get; set; }
        public int StudentCount { get; set; }
        public PlatformManagementSystem.Domain.Enums.CourseStatus Status { get; set; }

    }
}