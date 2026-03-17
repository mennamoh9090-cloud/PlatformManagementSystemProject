namespace PlatformManagementSystem.MVC.ViewModels.InstructorDashboard
{
    public class CourseDetailsVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public List<LessonVM> Lessons { get; set; }
    }

}
