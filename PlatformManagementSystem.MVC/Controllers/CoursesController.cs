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

        var token = HttpContext.Session.GetString("UserToken");
        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

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

    // ================== REVIEWS ==================
    [HttpPost]
    public async Task<IActionResult> AddReview(int courseId, int rating, string comment)
    {
        var client = _httpClientFactory.CreateClient("API");
        var token = HttpContext.Session.GetString("UserToken");
        if (string.IsNullOrWhiteSpace(token))
            return RedirectToAction("Login", "Account");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var reviewDto = new { CourseId = courseId, Rating = rating, Comment = comment };
        var json = JsonSerializer.Serialize(reviewDto);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await client.PostAsync("Review", content);
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            TempData["Error"] = "Could not submit review: " + err;
        }
        else
        {
            TempData["Success"] = "Review submitted successfully!";
        }

        return RedirectToAction("Details", new { id = courseId });
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

    // ================== ROLE REDIRECTS ==================
    [HttpGet]
    public IActionResult Create()
    {
        if (User.IsInRole("Instructor") || User.IsInRole("Admin"))
            return RedirectToAction("CreateCourse", "InstructorDashboard");

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        if (User.IsInRole("Instructor") || User.IsInRole("Admin"))
            return RedirectToAction("EditCourse", "InstructorDashboard", new { id });

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult MyCourses()
    {
        if (User.IsInRole("Instructor"))
            return RedirectToAction("MyCourses", "InstructorDashboard");

        if (User.IsInRole("Student"))
            return RedirectToAction("Index", "StudentDashboard");

        return RedirectToAction("Index");
    }
}
