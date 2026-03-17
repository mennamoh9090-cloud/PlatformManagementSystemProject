using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlatformManagementSystem.Application.DTOs.Dashboard;
namespace PlatformManagementSystem.Application.Interfaces;

public interface IDashboardService
{
    Task<object> GetAdminDashboardAsync();
    Task<object> GetInstructorDashboardAsync(string instructorId);
    Task<object> GetStudentDashboardAsync(string studentId);

    Task<object> GetStudentStatsAsync(string studentId);
    Task<object> GetMyCoursesDetailedAsync(string studentId);
    Task<object> GetMyExamResultsAsync(string studentId);

    Task<object?> GetCourseDetailsAsync(string studentId, int courseId);

    Task<byte[]?> GenerateCertificateAsync(string studentId, int courseId);
}
