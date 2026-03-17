
namespace PlatformManagementSystem.MVC.ViewModels.Student
{
    public class CourseCardVM
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public int ProgressPercentage { get; set; }
        public bool IsCompleted { get; set; }
        public List<LessonVM> Lessons { get; internal set; }
    }
}
