using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PlatformManagementSystem.MVC.ViewModels.Course;
using PlatformManagementSystem.MVC.ViewModels.Enrollment;
using PlatformManagementSystem.MVC.ViewModels.Exam;
using PlatformManagementSystem.MVC.ViewModels.InstructorDashboard;
using PlatformManagementSystem.MVC.ViewModels.Live;
using System.Security.Claims;
using System.Text.Json.Serialization;

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

        var totalLiveSessions = 0;

        if (courses != null && courses.Any())
        {
            foreach (var course in courses)
            {
                var sessions = await _apiService
                    .GetAsync<List<LiveSessionVM>>(
                        $"LiveSession/ByCourse/{course.CourseId}");

                if (sessions != null)
                {
                    totalLiveSessions += sessions.Count(s => s.IsActive);
                }
            }
        }

        var vm = new InstructorDashboardVM
        {
            MyCourses = courses ?? new List<CourseVM>(),
            TotalCourses = courses?.Count ?? 0,
            TotalStudents = courses?.Sum(c => c.StudentCount) ?? 0,
            TotalLiveSessions = totalLiveSessions
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCourse(CreateCourseVM model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCategories(model);
            return View(model);
        }

        var dto = new
        {
            Title = model.Title,
            Description = model.Description,
            Price = model.Price,
            CategoryId = model.CategoryId,
            ThumbnailUrl = model.ThumbnailUrl ?? ""
        };

        var result = await _apiService.PostAsync("Course", dto);

        if (!result)
        {
            TempData["Error"] = "Something went wrong!";
            await PopulateCategories(model);
            return View(model);
        }

        TempData["Success"] = "Course created successfully!";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> CreateCourse()
    {
        var model = new CreateCourseVM();
        await PopulateCategories(model);
        return View(model);
    }
    
    private async Task PopulateCategories(CreateCourseVM model)
    {
        var categories = await _apiService.GetAsync<List<CategoryResponse>>("Category");
        if (categories != null)
        {
            model.Categories = categories.Select(c => new SelectListItem 
            { 
                Value = c.Id.ToString(), 
                Text = c.Name 
            }).ToList();
        }
        else
        {
            model.Categories = new List<SelectListItem>();
        }
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
        var details = await _apiService.GetAsync<CourseDetailsResponse>($"Course/InstructorDetails/{courseId}");
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
    
    private sealed class CategoryResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}