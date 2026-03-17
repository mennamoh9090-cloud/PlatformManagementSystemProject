using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlatformManagementSystem.MVC.ViewModels.Live;
using PlatformManagementSystem.MVC.ViewModels.Student;

[Authorize(Roles = "Student")]
public class StudentDashboardController : Controller
{
    private readonly IApiService _apiService;

    public StudentDashboardController(IApiService apiService)
    {
        _apiService = apiService;
    }

    // =========================
    // Dashboard
    // =========================
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (!User.Identity!.IsAuthenticated)
            return RedirectToAction("Login", "Account");

        var stats = await _apiService.GetAsync<StudentStatsResponse>("Dashboard/StudentStats");
        var courses = await _apiService.GetAsync<List<StudentCourseResponse>>("Dashboard/MyCoursesDetailed");
        var examResults = await _apiService.GetAsync<List<StudentExamResultResponse>>("Dashboard/MyExamResults");

        var mappedCourses = (courses ?? new List<StudentCourseResponse>())
            .Select(c => new CourseCardVM
            {
                CourseId = c.CourseId,
                CourseTitle = c.CourseTitle ?? "Untitled",
                ProgressPercentage = c.ProgressPercentage,
                IsCompleted = c.IsCompleted
            })
            .ToList();

        var model = new StudentDashboardVM
        {
            TotalCourses = stats?.TotalCourses ?? mappedCourses.Count,
            CompletedCourses = stats?.CompletedCourses ?? mappedCourses.Count(c => c.IsCompleted),
            PendingCourses = stats?.PendingCourses ?? mappedCourses.Count(c => !c.IsCompleted),
            Courses = mappedCourses,
            Progress = mappedCourses
                .Select(c => new StudentProgressVM
                {
                    CourseTitle = c.CourseTitle,
                    ProgressPercentage = c.ProgressPercentage
                })
                .ToList(),
            ExamScores = (examResults ?? new List<StudentExamResultResponse>())
                .Select(e => new ExamScoreVM
                {
                    CourseTitle = e.CourseTitle ?? "Course",
                    ExamTitle = e.ExamTitle ?? "Exam",
                    Score = e.Score
                })
                .ToList()
        };

        return View(model);
    }

    // =========================
    // Start Learning
    // =========================
    [HttpGet("StartLearning/{courseId}")]
    public IActionResult StartLearning(int courseId)
    {
        return RedirectToAction("CourseDetails", new { courseId });
    }

    // =========================
    // Course Details
    // =========================
    [HttpGet("CourseDetails/{courseId}")]
    public async Task<IActionResult> CourseDetails(int courseId)
    {
        var course = await _apiService.GetAsync<StudentCourseDetailsResponse>($"Dashboard/CourseDetails/{courseId}");

        if (course == null)
            return RedirectToAction("Index");

        var model = new CourseDetailsVM
        {
            CourseId = course.CourseId,
            CourseTitle = course.CourseTitle ?? "Untitled",
            ProgressPercentage = course.ProgressPercentage,
            IsCompleted = course.IsCompleted,
            Lessons = (course.Lessons ?? new List<StudentLessonResponse>())
                .Select(l => new LessonVM
                {
                    LessonId = l.LessonId,
                    LessonTitle = l.LessonTitle ?? "Lesson",
                    IsCompleted = l.IsCompleted
                })
                .ToList()
        };

        var liveSessions = await _apiService
            .GetAsync<List<LiveSessionVM>>($"LiveSession/ByCourse/{courseId}");

        var activeSession = liveSessions?
            .FirstOrDefault(s => s.IsActive);

        ViewBag.ActiveLiveSessionId = activeSession?.Id ?? 0;

        return View(model);
    }

    [HttpGet("ProgressChart")]
    public async Task<IActionResult> ProgressChart()
    {
        var courses = await _apiService.GetAsync<List<StudentCourseResponse>>("Dashboard/MyCoursesDetailed");

        var data = (courses ?? new List<StudentCourseResponse>())
            .Select(c => new StudentProgressVM
            {
                CourseTitle = c.CourseTitle ?? "Untitled",
                ProgressPercentage = c.ProgressPercentage
            })
            .ToList();

        return View(data);
    }

    [HttpGet("Certificate/{courseId}")]
    public async Task<IActionResult> Certificate(int courseId)
    {
        var courses = await _apiService.GetAsync<List<StudentCourseResponse>>("Dashboard/MyCoursesDetailed");

        var course = (courses ?? new List<StudentCourseResponse>())
            .FirstOrDefault(c => c.CourseId == courseId);

        if (course == null || !course.IsCompleted)
            return RedirectToAction("Index");

        ViewBag.StudentName = User.Identity?.Name ?? "Student";

        return View(new CourseCardVM
        {
            CourseId = course.CourseId,
            CourseTitle = course.CourseTitle ?? "Untitled",
            IsCompleted = course.IsCompleted
        });
    }

    private sealed class StudentStatsResponse
    {
        public int TotalCourses { get; set; }
        public int CompletedCourses { get; set; }
        public int PendingCourses { get; set; }
    }

    private sealed class StudentCourseResponse
    {
        public int CourseId { get; set; }
        public string? CourseTitle { get; set; }
        public int ProgressPercentage { get; set; }
        public bool IsCompleted { get; set; }
    }

    private sealed class StudentExamResultResponse
    {
        public string? CourseTitle { get; set; }
        public string? ExamTitle { get; set; }
        public double Score { get; set; }
    }

    private sealed class StudentCourseDetailsResponse
    {
        public int CourseId { get; set; }
        public string? CourseTitle { get; set; }
        public int ProgressPercentage { get; set; }
        public bool IsCompleted { get; set; }
        public List<StudentLessonResponse>? Lessons { get; set; }
    }

    private sealed class StudentLessonResponse
    {
        public int LessonId { get; set; }
        public string? LessonTitle { get; set; }
        public bool IsCompleted { get; set; }
    }
}