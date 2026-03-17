using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PlatformManagementSystem.Application.Interfaces;
using PlatformManagementSystem.Application.Interfaces.Services;
using PlatformManagementSystem.API.Hubs;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class LiveSessionController : ControllerBase
{
    private readonly ILiveSessionService _liveService;
    private readonly IHubContext<LiveSessionHub> _hub;
    private readonly IUnitOfWork _unitOfWork;

    public LiveSessionController(
        ILiveSessionService liveService,
        IHubContext<LiveSessionHub> hub,
        IUnitOfWork unitOfWork)
    {
        _liveService = liveService;
        _hub = hub;
        _unitOfWork = unitOfWork;
    }

    // Create Session
    [Authorize(Roles = "Instructor")]
    [HttpPost("Create")]
    public async Task<IActionResult> Create(
        int courseId,
        string title,
        DateTime startTime,
        string meetingUrl)
    {
        var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(instructorId))
            return Unauthorized();

        var id = await _liveService.CreateSessionAsync(instructorId, courseId, title, startTime);
        return Ok(id);
    }

    // Get Sessions By Course
    [HttpGet("ByCourse/{courseId}")]
    public async Task<IActionResult> GetByCourse(int courseId)
    {
        var sessions = await _liveService.GetByCourseAsync(courseId);
        return Ok(sessions);
    }

    // Start Session
    [Authorize(Roles = "Instructor")]
    [HttpPost("Start/{id}")]
    public async Task<IActionResult> Start(int id, int courseId)
    {
        await _liveService.StartSessionAsync(id);

        await _hub.Clients.Group($"Course_{courseId}")
            .SendAsync("SessionStarted", courseId);

        return Ok();
    }

    // End Session
    [Authorize(Roles = "Instructor")]
    [HttpPost("End/{id}")]
    public async Task<IActionResult> End(int id)
    {
        await _liveService.EndSessionAsync(id);
        return Ok();
    }

    /// <summary>Get saved whiteboard events for a session (replay on load).</summary>
    [HttpGet("WhiteboardHistory/{sessionId}")]
    public async Task<IActionResult> WhiteboardHistory(int sessionId)
    {
        var events = await _unitOfWork.WhiteboardEvents
            .FindAsync(e => e.LiveSessionId == sessionId);
        var list = events
            .OrderBy(e => e.CreatedAt)
            .Where(e => e.EventType == "Draw" && !string.IsNullOrEmpty(e.Data))
            .Select(e => new { data = e.Data })
            .ToList();
        return Ok(list);
    }
}