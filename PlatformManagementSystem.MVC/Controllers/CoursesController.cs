using Microsoft.AspNetCore.Mvc;
using PlatformManagementSystem.MVC.ViewModels.Course;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PlatformManagementSystem.MVC.Controllers;

public class CoursesController(IHttpClientFactory httpClientFactory) : Controller
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    // ================== INDEX ==================
    public async Task<IActionResult> Index(string? search)
    {
        var client = httpClientFactory.CreateClient("API");

        var response = await client.GetAsync("Course");

        if (!response.IsSuccessStatusCode)
            return View(new List<CourseViewModel>());

        var json = await response.Content.ReadAsStringAsync();

        var courses = JsonSerializer.Deserialize<List<CourseViewModel>>(json, _jsonOptions) ?? [];

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            courses = courses
                .Where(c =>
                    (!string.IsNullOrWhiteSpace(c.Title) && c.Title.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(c.Description) && c.Description.Contains(term, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        ViewBag.Search = search ?? string.Empty;

        return View(courses);
    }

    // ================== DETAILS ==================
    public async Task<IActionResult> Details(int id)
    {
        var client = httpClientFactory.CreateClient("API");

        var response = await client.GetAsync($"Course/details/{id}");

        if (!response.IsSuccessStatusCode)
            return NotFound();

        var json = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(json))
            return NotFound();

        var course = JsonSerializer.Deserialize<CourseDetailsViewModel>(json, _jsonOptions);

        if (course == null)
            return NotFound();

        return View(course);
    }

    // ================== ENROLL ==================
    [HttpPost]
    public async Task<IActionResult> Enroll(int courseId)
    {
        var client = httpClientFactory.CreateClient("API");

        var token = HttpContext.Session.GetString("UserToken");

        if (string.IsNullOrWhiteSpace(token))
            return RedirectToAction("Login", "Account");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        await client.PostAsync($"Enrollment/{courseId}", null);

        return RedirectToAction("Details", new { id = courseId });
    }
}
