using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlatformManagementSystem.MVC.ViewModels.Live;
using System.Text.Json;

[Authorize]
public class LiveSessionController : Controller
{
    private readonly IHttpClientFactory _factory;

    public LiveSessionController(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task<IActionResult> ByCourse(int courseId)
    {
        var client = _factory.CreateClient("API");

        var response = await client.GetAsync($"LiveSession/ByCourse/{courseId}");
        if (!response.IsSuccessStatusCode)
        {
            ViewBag.CourseId = courseId;
            return View(new List<LiveSessionVM>());
        }

        var json = await response.Content.ReadAsStringAsync();

        var sessions = JsonSerializer.Deserialize<List<LiveSessionVM>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        ViewBag.CourseId = courseId;
        return View(sessions ?? new List<LiveSessionVM>());
    }

    [HttpPost]
    public async Task<IActionResult> Create(int courseId, string title, DateTime startTime, string meetingUrl)
    {
        var client = _factory.CreateClient("API");

        var encodedTitle = Uri.EscapeDataString(title ?? string.Empty);
        var encodedMeetingUrl = Uri.EscapeDataString(meetingUrl ?? string.Empty);
        var response = await client.PostAsync(
            $"LiveSession/Create?courseId={courseId}&title={encodedTitle}&startTime={startTime:o}&meetingUrl={encodedMeetingUrl}",
            null);

        if (!response.IsSuccessStatusCode)
            TempData["Error"] = "Failed to create live session.";

        return RedirectToAction("ByCourse", new { courseId });
    }

    [HttpPost]
    public async Task<IActionResult> Start(int id, int courseId)
    {
        var client = _factory.CreateClient("API");
        var response = await client.PostAsync($"LiveSession/Start/{id}?courseId={courseId}", null);

        if (!response.IsSuccessStatusCode)
            TempData["Error"] = "Failed to start live session.";
        else
            TempData["Success"] = "Live session started.";

        return RedirectToAction(nameof(Board), new { sessionId = id, courseId });
    }

    [HttpPost]
    public async Task<IActionResult> End(int id, int courseId)
    {
        var client = _factory.CreateClient("API");
        var response = await client.PostAsync($"LiveSession/End/{id}", null);

        if (!response.IsSuccessStatusCode)
            TempData["Error"] = "Failed to end live session.";
        else
            TempData["Success"] = "Live session ended.";

        return RedirectToAction("ByCourse", new { courseId });
    }

    [HttpGet]
    public IActionResult Session(int sessionId, int courseId)
    {
        ViewBag.SessionId = sessionId;
        ViewBag.CourseId = courseId;
        ViewBag.Token = HttpContext.Session.GetString("UserToken") ?? string.Empty;
        return View();
    }

    [HttpGet]
    public IActionResult Board(int sessionId, int courseId)
    {
        ViewBag.SessionId = sessionId;
        ViewBag.CourseId = courseId;
        ViewBag.Token = HttpContext.Session.GetString("UserToken") ?? string.Empty;
        return View();
    }
}