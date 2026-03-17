using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlatformManagementSystem.Application.Interfaces;
using PlatformManagementSystem.Infrastructure.Repositories;
using System.Security.Claims;

namespace PlatformManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly IUnitOfWork _unitOfWork;

    public DashboardController(IDashboardService dashboardService, IUnitOfWork unitOfWork)
    {
        _dashboardService = dashboardService;
        _unitOfWork = unitOfWork;
    }
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAdminDashboard()
    {
        var result = await _dashboardService.GetAdminDashboardAsync();
        return Ok(result);
    }

    [HttpGet("instructor")]
    [Authorize(Roles = "Instructor")]
    public async Task<IActionResult> GetInstructorDashboard()
    {
        var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(instructorId))
            return Unauthorized();

        var result = await _dashboardService
            .GetInstructorDashboardAsync(instructorId);

        return Ok(result);
    }

    [HttpGet("student")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetStudentDashboard()
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(studentId))
            return Unauthorized();

        var result = await _dashboardService
            .GetStudentDashboardAsync(studentId);

        return Ok(result);
    }

    [Authorize(Roles = "Student")]
    [HttpGet("StudentStats")]
    public async Task<IActionResult> GetStudentStats()
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(studentId))
            return Unauthorized();

        var result = await _dashboardService.GetStudentStatsAsync(studentId);

        return Ok(result);
    }

    [Authorize(Roles = "Student")]
    [HttpGet("MyCoursesDetailed")]
    public async Task<IActionResult> GetMyCoursesDetailed()
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(studentId))
            return Unauthorized();

        var result = await _dashboardService.GetMyCoursesDetailedAsync(studentId);

        return Ok(result);
    }

    [Authorize(Roles = "Student")]
    [HttpGet("MyExamResults")]
    public async Task<IActionResult> GetMyExamResults()
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(studentId))
            return Unauthorized();

        var result = await _dashboardService.GetMyExamResultsAsync(studentId);

        return Ok(result);
    }

    [Authorize(Roles = "Student")]
    [HttpGet("CourseDetails/{courseId}")]
    public async Task<IActionResult> GetCourseDetails(int courseId)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(studentId))
            return Unauthorized();

        var result = await _dashboardService
            .GetCourseDetailsAsync(studentId, courseId);

        if (result == null)
            return BadRequest("Not enrolled in this course");

        return Ok(result);
    }

    [Authorize(Roles = "Student")]
    [HttpGet("GenerateCertificate/{courseId}")]
    public async Task<IActionResult> GenerateCertificate(int courseId)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(studentId))
            return Unauthorized();

        var result = await _dashboardService
            .GenerateCertificateAsync(studentId, courseId);

        if (result == null)
            return BadRequest("Course not completed yet");

        return File(result, "application/pdf", "Certificate.pdf");
    }
    [Authorize(Roles = "Student")]
    [HttpGet("StudentProgress")]
    public async Task<IActionResult> GetStudentProgress()
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var enrollments = await _unitOfWork.Enrollments
            .FindAsync(e => e.StudentId == studentId);

        var result = enrollments.Select(e => new
        {
            e.CourseId,
            Progress = e.ProgressPercentage
        });

        return Ok(result);
    }

    [Authorize(Roles = "Student")]
    [HttpGet("CompletionStats")]
    public async Task<IActionResult> GetCompletionStats()
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var enrollments = await _unitOfWork.Enrollments
            .FindAsync(e => e.StudentId == studentId);

        var completed = enrollments.Count(e => e.IsCompleted);
        var remaining = enrollments.Count(e => !e.IsCompleted);

        return Ok(new { completed, remaining });
    }

}

