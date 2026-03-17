using PlatformManagementSystem.MVC.ViewModels.Instructor;

namespace PlatformManagementSystem.MVC.ViewModels.InstructorDashboard
{
    public class InstructorDashboardVM
    {
        public int TotalCourses { get; set; }
        public int TotalStudents { get; set; }
        public int TotalLiveSessions { get; set; }

        public List<CourseVM> MyCourses { get; set; } = new();
    }
}