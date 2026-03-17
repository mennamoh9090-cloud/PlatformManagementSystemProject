namespace PlatformManagementSystem.MVC.ViewModels.Instructor
{
    public class CourseStatsVM
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public int LessonsCount { get; set; }
        public int StudentsCount { get; set; }
    }
}