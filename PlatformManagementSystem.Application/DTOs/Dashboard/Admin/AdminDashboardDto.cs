
namespace PlatformManagementSystem.Application.DTOs.Dashboard;

public class AdminDashboardDto
{
    public int TotalUsers { get; set; }
    public int TotalCourses { get; set; }
    public int TotalEnrollments { get; set; }
    public int TotalInstructors { get; set; }
    public int TotalStudents { get; set; }
    public int ApprovedCourses { get; internal set; }
}
