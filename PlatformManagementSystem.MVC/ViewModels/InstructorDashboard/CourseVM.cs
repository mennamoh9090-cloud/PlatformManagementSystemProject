namespace PlatformManagementSystem.MVC.ViewModels.InstructorDashboard
{
    public class CourseVM
    {
        public int CourseId { get; set; }
        public string Title { get; set; }

        public string Description { get; set; } 
        public int LessonsCount { get; set; }
        public int StudentCount { get; set; }

    }
}