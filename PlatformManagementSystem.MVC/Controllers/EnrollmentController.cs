using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Student")]
public class EnrollmentsController : Controller
{
    private readonly IApiService _apiService;

    public EnrollmentsController(IApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpPost]
    public async Task<IActionResult> Enroll(int courseId)
    {
        var success = await _apiService.PostAsync<object?>($"Enrollment/{courseId}", null);

        if (!success)
        {
            TempData["Error"] = "Enrollment failed or you are already enrolled in this course.";
            return RedirectToAction("Details", "Courses", new { id = courseId });
        }

        TempData["Success"] = "Enrollment successful!";
        return RedirectToAction("CourseDetails", "StudentDashboard", new { courseId });
    }
}
