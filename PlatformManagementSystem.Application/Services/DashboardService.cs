using Microsoft.AspNetCore.Identity;
using PlatformManagementSystem.Application.DTOs.Dashboard;
using PlatformManagementSystem.Application.Interfaces;
using PlatformManagementSystem.Domain.Entities;
using PlatformManagementSystem.Domain.Enums;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardService(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<byte[]?> GenerateCertificateAsync(string studentId, int courseId)
    {
        var enrollment = await _unitOfWork.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

        if (enrollment == null || !enrollment.IsCompleted)
            return null;

        return null;
    }

    public async Task<object> GetAdminDashboardAsync()
    {
        var users = _userManager.Users.ToList();

        int students = 0;
        int instructors = 0;

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Student"))
                students++;

            if (roles.Contains("Instructor"))
                instructors++;
        }

        var courses = await _unitOfWork.Courses.GetAllAsync();

        return new AdminDashboardDto
        {
            TotalStudents = students,
            TotalInstructors = instructors,
            TotalCourses = courses.Count(),
            ApprovedCourses = courses.Count(c => c.Status == CourseStatus.Approved)
        };
    }

    public async Task<object?> GetCourseDetailsAsync(string studentId, int courseId)
    {
        var enrollment = await _unitOfWork.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

        if (enrollment == null)
            return null;

        var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
        var lessons = (await _unitOfWork.Lessons.FindAsync(l => l.CourseId == courseId)).ToList();
        var progress = (await _unitOfWork.StudentLessonProgresses
            .FindAsync(p => p.StudentId == studentId && p.CourseId == courseId))
            .ToList();

        var lessonProgressById = progress.ToDictionary(p => p.LessonId, p => p.IsCompleted);

        return new
        {
            CourseId = courseId,
            CourseTitle = course?.Title ?? "Untitled",
            ProgressPercentage = enrollment.ProgressPercentage,
            IsCompleted = enrollment.IsCompleted,
            Lessons = lessons.Select(l => new
            {
                LessonId = l.Id,
                LessonTitle = l.Title,
                IsCompleted = lessonProgressById.TryGetValue(l.Id, out var isCompleted) && isCompleted
            })
        };
    }

    public async Task<object> GetInstructorDashboardAsync(string instructorId)
    {
        var courses = await _unitOfWork.Courses
            .FindAsync(c => c.InstructorId == instructorId);

        var enrollments = await _unitOfWork.Enrollments.GetAllAsync();

        int studentsCount = enrollments
            .Count(e => courses.Any(c => c.Id == e.CourseId));

        return new InstructorDashboardDto
        {
            MyCoursesCount = courses.Count(),
            TotalStudentsInCourses = studentsCount
        };
    }

    public async Task<object> GetMyCoursesDetailedAsync(string studentId)
    {
        var enrollments = (await _unitOfWork.Enrollments
            .FindAsync(e => e.StudentId == studentId))
            .ToList();

        if (enrollments.Count == 0)
            return Array.Empty<object>();

        var courses = (await _unitOfWork.Courses.GetAllAsync()).ToList();
        var courseTitles = courses.ToDictionary(c => c.Id, c => c.Title);

        return enrollments.Select(e => new
        {
            CourseId = e.CourseId,
            CourseTitle = courseTitles.TryGetValue(e.CourseId, out var title) ? title : "Untitled",
            ProgressPercentage = e.ProgressPercentage,
            IsCompleted = e.IsCompleted
        });
    }

    public async Task<object> GetMyExamResultsAsync(string studentId)
    {
        var studentExams = (await _unitOfWork.StudentExams
            .FindAsync(se => se.StudentId == studentId))
            .ToList();

        if (studentExams.Count == 0)
            return Array.Empty<object>();

        var exams = (await _unitOfWork.Exams.GetAllAsync()).ToList();
        var courses = (await _unitOfWork.Courses.GetAllAsync()).ToList();

        var examById = exams.ToDictionary(e => e.Id);
        var courseById = courses.ToDictionary(c => c.Id);

        return studentExams.Select(se =>
        {
            examById.TryGetValue(se.ExamId, out var exam);
            var courseTitle = "Unknown Course";

            if (exam != null && courseById.TryGetValue(exam.CourseId, out var course))
                courseTitle = course.Title;

            return new
            {
                CourseTitle = courseTitle,
                ExamTitle = exam?.Title ?? "Exam",
                Score = se.Score
            };
        });
    }

    public async Task<object> GetStudentDashboardAsync(string studentId)
    {
        var enrollments = await _unitOfWork.Enrollments
            .FindAsync(e => e.StudentId == studentId);

        return new StudentDashboardDto
        {
            EnrolledCoursesCount = enrollments.Count(),
            CompletedCoursesCount = enrollments.Count(e => e.IsCompleted)
        };
    }

    public async Task<object> GetStudentStatsAsync(string studentId)
    {
        var enrollments = (await _unitOfWork.Enrollments
            .FindAsync(e => e.StudentId == studentId))
            .ToList();

        var totalCourses = enrollments.Count;
        var completedCourses = enrollments.Count(e => e.IsCompleted);

        return new
        {
            TotalCourses = totalCourses,
            CompletedCourses = completedCourses,
            PendingCourses = Math.Max(0, totalCourses - completedCourses)
        };
    }
}

