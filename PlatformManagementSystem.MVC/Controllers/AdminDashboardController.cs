using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlatformManagementSystem.MVC.Filters;
using PlatformManagementSystem.MVC.ViewModels.Admin;
using System.Text.Json;

[AuthorizeSession]
[Authorize(Roles = "Admin")]
public class AdminDashboardController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AdminDashboardController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var client = _httpClientFactory.CreateClient("API");

        var token = HttpContext.Session.GetString("UserToken");

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        var response = await client.GetAsync("Admin/PlatformStats");

        if (!response.IsSuccessStatusCode)
        {
            return View(new AdminStatsVM());
        }
       var json = await response.Content.ReadAsStringAsync();

        var stats = JsonSerializer.Deserialize<AdminStatsVM>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(stats);
    }

    public async Task<IActionResult> Users()
    {
        var client = _httpClientFactory.CreateClient("API");

        var token = HttpContext.Session.GetString("UserToken");

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        var response = await client.GetAsync("Admin/Users");
        if (!response.IsSuccessStatusCode)
            return View(new List<AdminUserVM>());

        var json = await response.Content.ReadAsStringAsync();

        var model = JsonSerializer.Deserialize<List<AdminUserVM>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(model ?? new List<AdminUserVM>());
    }

    public async Task<IActionResult> Courses()
    {
        var client = _httpClientFactory.CreateClient("API");

        var token = HttpContext.Session.GetString("UserToken");

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        var response = await client.GetAsync("Admin/Courses");
        if (!response.IsSuccessStatusCode)
            return View(new List<AdminCourseVM>());

        var json = await response.Content.ReadAsStringAsync();

        var model = JsonSerializer.Deserialize<List<AdminCourseVM>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(model ?? new List<AdminCourseVM>());
    }

    [HttpGet]
    public async Task<IActionResult> DeleteUser(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return RedirectToAction(nameof(Users));

        var client = _httpClientFactory.CreateClient("API");
        var token = HttpContext.Session.GetString("UserToken");

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        await client.DeleteAsync($"Admin/DeleteUser/{id}");
        return RedirectToAction(nameof(Users));
    }
}