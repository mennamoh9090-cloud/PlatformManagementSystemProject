using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlatformManagementSystem.MVC.Filters;
using PlatformManagementSystem.MVC.ViewModels.Admin;
using System.Text.Json;

namespace PlatformManagementSystem.MVC.Controllers;

[AuthorizeSession]
[Authorize(Roles = "Admin")]
public class AdminDashboardController(IHttpClientFactory httpClientFactory) : Controller
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<IActionResult> Index()
    {
        var client = httpClientFactory.CreateClient("API");

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

        var stats = JsonSerializer.Deserialize<AdminStatsVM>(json, _jsonOptions);

        return View(stats);
    }

    public async Task<IActionResult> Users()
    {
        var client = httpClientFactory.CreateClient("API");

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

        var model = JsonSerializer.Deserialize<List<AdminUserVM>>(json, _jsonOptions);

        return View(model ?? []);
    }

    public async Task<IActionResult> Courses()
    {
        var client = httpClientFactory.CreateClient("API");

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

        var model = JsonSerializer.Deserialize<List<AdminCourseVM>>(json, _jsonOptions);

        return View(model ?? []);
    }

    [HttpGet]
    public async Task<IActionResult> DeleteUser(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return RedirectToAction(nameof(Users));

        var client = httpClientFactory.CreateClient("API");
        var token = HttpContext.Session.GetString("UserToken");

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        await client.DeleteAsync($"Admin/DeleteUser/{id}");
        return RedirectToAction(nameof(Users));
    }

    public async Task<IActionResult> Roles()
    {
        var client = httpClientFactory.CreateClient("API");
        var token = HttpContext.Session.GetString("UserToken");

        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("Admin/Roles");
        if (!response.IsSuccessStatusCode)
            return View(new List<AdminRoleVM>());

        var json = await response.Content.ReadAsStringAsync();
        var roles = JsonSerializer.Deserialize<List<AdminRoleVM>>(json, _jsonOptions);

        return View(roles ?? []);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName)) return RedirectToAction(nameof(Roles));

        var client = httpClientFactory.CreateClient("API");
        var token = HttpContext.Session.GetString("UserToken");

        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var content = new StringContent(JsonSerializer.Serialize(new { RoleName = roleName }), System.Text.Encoding.UTF8, "application/json");
        await client.PostAsync("Admin/CreateRole", content);

        return RedirectToAction(nameof(Roles));
    }

    public async Task<IActionResult> DeleteRole(string roleName)
    {
        var client = httpClientFactory.CreateClient("API");
        var token = HttpContext.Session.GetString("UserToken");

        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        await client.DeleteAsync($"Admin/DeleteRole/{roleName}");

        return RedirectToAction(nameof(Roles));
    }

    public async Task<IActionResult> ManageUserRoles(string id)
    {
        var client = httpClientFactory.CreateClient("API");
        var token = HttpContext.Session.GetString("UserToken");

        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var usersResponse = await client.GetAsync("Admin/Users");
        var usersJson = await usersResponse.Content.ReadAsStringAsync();
        var users = JsonSerializer.Deserialize<List<AdminUserVM>>(usersJson, _jsonOptions);

        var user = users?.FirstOrDefault(u => u.Id == id);
        if (user == null) return RedirectToAction(nameof(Users));

        var rolesResponse = await client.GetAsync("Admin/Roles");
        var rolesJson = await rolesResponse.Content.ReadAsStringAsync();
        var allRoles = JsonSerializer.Deserialize<List<AdminRoleVM>>(rolesJson, _jsonOptions);

        var model = new ManageUserRolesVM
        {
            UserId = user.Id,
            FullName = user.FullName,
            UserRoles = user.Roles ?? [],
            AvailableRoles = allRoles ?? []
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AssignRole(string userId, string roleName)
    {
        var client = httpClientFactory.CreateClient("API");
        var token = HttpContext.Session.GetString("UserToken");

        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var dto = new AssignRoleVM { UserId = userId, RoleName = roleName };
        var content = new StringContent(JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");

        await client.PostAsync("Admin/AssignRole", content);

        return RedirectToAction(nameof(ManageUserRoles), new { id = userId });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveRole(string userId, string roleName)
    {
        var client = httpClientFactory.CreateClient("API");
        var token = HttpContext.Session.GetString("UserToken");

        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var dto = new AssignRoleVM { UserId = userId, RoleName = roleName };
        var content = new StringContent(JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");

        await client.PostAsync("Admin/RemoveRole", content);

        return RedirectToAction(nameof(ManageUserRoles), new { id = userId });
    }

    public async Task<IActionResult> DeleteCourse(int id)
    {
        var client = httpClientFactory.CreateClient("API");
        var token = HttpContext.Session.GetString("UserToken");

        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        await client.DeleteAsync($"Admin/DeleteCourse/{id}");
        return RedirectToAction(nameof(Courses));
    }

    [HttpPost]
    public async Task<IActionResult> ApproveCourse(int id)
    {
        var client = httpClientFactory.CreateClient("API");
        var token = HttpContext.Session.GetString("UserToken");

        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        await client.PostAsync($"Admin/ApproveCourse/{id}", null);
        return RedirectToAction(nameof(Courses));
    }

    [HttpPost]
    public async Task<IActionResult> RejectCourse(int id)
    {
        var client = httpClientFactory.CreateClient("API");
        var token = HttpContext.Session.GetString("UserToken");

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        await client.PostAsync($"Admin/RejectCourse/{id}", null);
        return RedirectToAction(nameof(Courses));
    }
}