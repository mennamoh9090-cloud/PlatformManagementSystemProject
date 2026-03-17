namespace PlatformManagementSystem.MVC.ViewModels.Student
{
    public class CourseDetailsVM
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public int ProgressPercentage { get; set; }
        public bool IsCompleted { get; set; }

        public List<LessonVM> Lessons { get; set; } = new();
    }

    public class LessonVM
    {
        public int LessonId { get; set; }
        public string LessonTitle { get; set; }
        public bool IsCompleted { get; set; }
    }
}