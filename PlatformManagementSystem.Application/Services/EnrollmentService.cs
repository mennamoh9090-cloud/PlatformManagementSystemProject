using PlatformManagementSystem.Application.Interfaces;
using PlatformManagementSystem.Domain.Entities;

namespace PlatformManagementSystem.Application.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EnrollmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Enroll Student
        public async Task EnrollStudentAsync(string studentId, int courseId)
        {
            var existingEnrollments = await _unitOfWork.Enrollments
                .FindAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (existingEnrollments.Any())
                throw new Exception("Student already enrolled in this course");

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.UtcNow,
                ProgressPercentage = 0,
                IsCompleted = false
            };

            await _unitOfWork.Enrollments.AddAsync(enrollment);
            await _unitOfWork.SaveChangesAsync();
        }

        // Get Student Courses
        public async Task<IEnumerable<Enrollment>> GetStudentEnrollmentsAsync(string studentId)
        {
            return await _unitOfWork.Enrollments
                .FindAsync(e => e.StudentId == studentId);
        }

        // Get Student Count In Course
        public async Task<int> GetCourseStudentCountAsync(int courseId)
        {
            var enrollments = await _unitOfWork.Enrollments
                .FindAsync(e => e.CourseId == courseId);

            return enrollments.Count();
        }

        
        // Update Progress by EnrollmentId
        public async Task UpdateProgressAsync(int enrollmentId, int progress)
        {
            var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(enrollmentId);

            if (enrollment == null)
                throw new Exception("Enrollment not found");

            enrollment.ProgressPercentage = progress;
            enrollment.IsCompleted = progress >= 100;

            _unitOfWork.Enrollments.Update(enrollment);
            await _unitOfWork.SaveChangesAsync();
        }

        // Update Progress by Student + Course
        public async Task UpdateProgressAsync(string studentId, int courseId, int progress)
        {
            var enrollment = (await _unitOfWork.Enrollments
                .FindAsync(e => e.StudentId == studentId && e.CourseId == courseId))
                .FirstOrDefault();

            if (enrollment == null)
                throw new Exception("Enrollment not found");

            enrollment.ProgressPercentage = progress;
            enrollment.IsCompleted = progress >= 100;

            _unitOfWork.Enrollments.Update(enrollment);
            await _unitOfWork.SaveChangesAsync();
        }

        // Get Students By Course
        
        public async Task<IEnumerable<object>> GetStudentsByCourseAsync(int courseId)
        {
            var enrollments = await _unitOfWork.Enrollments
                .FindAsync(e => e.CourseId == courseId);

            return enrollments.Select(e => new
            {
                e.StudentId,
                e.EnrollmentDate,
                e.ProgressPercentage,
                e.IsCompleted
            });
        }

    }
}

