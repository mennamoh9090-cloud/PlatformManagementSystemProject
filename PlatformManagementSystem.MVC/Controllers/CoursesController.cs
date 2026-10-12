using Microsoft.AspNetCore.Mvc;
using PlatformManagementSystem.MVC.ViewModels.Course;
using System.Net.Http.Headers;
using System.Text.Json;

public class CoursesController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CoursesController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    // ================== INDEX ==================
    public async Task<IActionResult> Index()
    {
        var client = _httpClientFactory.CreateClient("API");

        var response = await client.GetAsync("Course");

        if (!response.IsSuccessStatusCode)
            return View(new List<CourseViewModel>());

        var json = await response.Content.ReadAsStringAsync();

        var courses = JsonSerializer.Deserialize<List<CourseViewModel>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(courses);
    }

    // ================== DETAILS ==================
    public async Task<IActionResult> Details(int id)
    {
        var client = _httpClientFactory.CreateClient("API");

        var response = await client.GetAsync($"Course/details/{id}");

        if (!response.IsSuccessStatusCode)
            return NotFound();

        var json = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(json))
            return NotFound();

        var course = JsonSerializer.Deserialize<CourseDetailsViewModel>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (course == null)
            return NotFound();

        return View(course);
    }

    // ================== ENROLL ==================
    [HttpPost]
    public async Task<IActionResult> Enroll(int courseId)
    {
        var client = _httpClientFactory.CreateClient("API");

        var token = HttpContext.Session.GetString("UserToken");

        if (string.IsNullOrWhiteSpace(token))
            return RedirectToAction("Login", "Account");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        await client.PostAsync($"Enrollment/{courseId}", null);

        return RedirectToAction("Details", new { id = courseId });
    }
}
