namespace PlatformManagementSystem.MVC.ViewModels.Student
{
    public class StudentDashboardVM
    {
        public int TotalCourses { get; set; }
        public int CompletedCourses { get; set; }
        public int PendingCourses { get; set; }

        public List<StudentProgressVM> Progress { get; set; } = new();
        public List<ExamScoreVM> ExamScores { get; set; } = new();

        public List<CourseCardVM>Courses { get; set; } =new();
    }
}
