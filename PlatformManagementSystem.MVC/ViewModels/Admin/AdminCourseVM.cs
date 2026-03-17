using PlatformManagementSystem.Domain.Enums;

namespace PlatformManagementSystem.MVC.ViewModels.Admin
{
    public class AdminCourseVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int StudentsCount { get; set; }
        public string InstructorName { get; set; }
        public CourseStatus Status { get; set; }
    }
}
