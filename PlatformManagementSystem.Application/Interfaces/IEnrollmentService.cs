using PlatformManagementSystem.Domain.Entities;

namespace PlatformManagementSystem.Application.Interfaces
{
    public interface IEnrollmentService
    {
        Task EnrollStudentAsync(string studentId, int courseId);
        Task<IEnumerable<Enrollment>> GetStudentEnrollmentsAsync(string studentId);
        Task<int> GetCourseStudentCountAsync(int courseId);
        Task UpdateProgressAsync(int enrollmentId, int progress);
        Task UpdateProgressAsync(string studentId, int courseId, int progress);
        Task<IEnumerable<object>> GetStudentsByCourseAsync(int courseId);


    }
}


