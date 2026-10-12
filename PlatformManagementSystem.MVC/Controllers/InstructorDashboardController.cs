using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlatformManagementSystem.MVC.ViewModels.Course;
using PlatformManagementSystem.MVC.ViewModels.Enrollment;
using PlatformManagementSystem.MVC.ViewModels.Exam;
using PlatformManagementSystem.MVC.ViewModels.InstructorDashboard;
using System.Security.Claims;

[Authorize(Roles = "Instructor")]
public class InstructorDashboardController : Controller
{
    private readonly IApiService _apiService;

    public InstructorDashboardController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var instructorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(instructorId))
            return RedirectToAction("Login", "Account");

        var courses = await _apiService
            .GetAsync<List<CourseVM>>(
                $"Course/ByInstructor/{instructorId}");

        var vm = new InstructorDashboardVM
        {
            MyCourses = courses ?? new List<CourseVM>(),
            TotalCourses = courses?.Count ?? 0,
            TotalStudents = courses?.Sum(c => c.StudentCount) ?? 0,
            TotalLiveSessions = 0
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCourse(CreateCourseVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var dto = new
        {
            Title = model.Title,
            Description = model.Description
        };

        var result = await _apiService.PostAsync("Course", dto);

        if (!result)
        {
            TempData["Error"] = "Something went wrong!";
            return View(model);
        }

        TempData["Success"] = "Course created successfully!";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult CreateCourse()
    {
        return View(new CreateCourseVM());
    }

    [HttpGet]
    public async Task<IActionResult> MyCourses()
    {
        var instructorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(instructorId))
            return RedirectToAction("Login", "Account");

        var courses = await _apiService.GetAsync<List<CourseVM>>($"Course/ByInstructor/{instructorId}");

        var model = (courses ?? new List<CourseVM>())
            .Select(c => new CourseViewModel
            {
                Id = c.CourseId,
                Title = c.Title,
                Description = c.Description
            })
            .ToList();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ManageCourse(int courseId)
    {
        var details = await _apiService.GetAsync<CourseDetailsResponse>($"Course/details/{courseId}");
        if (details == null)
            return RedirectToAction("Index");

        var model = new CourseDetailsVM
        {
            Id = details.Id,
            Title = details.Title ?? "Untitled",
            Description = details.Description ?? string.Empty,
            Lessons = (details.Lessons ?? new List<LessonResponse>())
                .Select(l => new LessonVM
                {
                    Id = l.Id,
                    Title = l.Title ?? "Lesson"
                })
                .ToList()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ViewStudents(int courseId)
    {
        var students = await _apiService.GetAsync<List<StudentByCourseResponse>>($"Enrollment/ByCourse/{courseId}");

        var model = (students ?? new List<StudentByCourseResponse>())
            .Select(s => new StudentProgressViewModel
            {
                Id = s.StudentId ?? string.Empty,
                FullName = s.StudentId ?? "Student",
                Progress = s.ProgressPercentage
            })
            .ToList();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ViewExams(int courseId)
    {
        var exams = await _apiService.GetAsync<List<ExamByCourseResponse>>($"Exam/ByCourse/{courseId}");

        var model = (exams ?? new List<ExamByCourseResponse>())
            .Select(e => new ExamViewModel
            {
                Id = e.Id,
                Title = e.Title ?? "Exam"
            })
            .ToList();

        return View(model);
    }

    private sealed class CourseDetailsResponse
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public List<LessonResponse>? Lessons { get; set; }
    }

    private sealed class LessonResponse
    {
        public int Id { get; set; }
        public string? Title { get; set; }
    }

    private sealed class StudentByCourseResponse
    {
        public string? StudentId { get; set; }
        public int ProgressPercentage { get; set; }
    }

    private sealed class ExamByCourseResponse
    {
        public int Id { get; set; }
        public string? Title { get; set; }
    }
}