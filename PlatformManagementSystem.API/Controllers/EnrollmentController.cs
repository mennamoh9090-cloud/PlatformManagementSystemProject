using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlatformManagementSystem.Application.Interfaces;
using System.Security.Claims;

namespace PlatformManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    // Enroll Student In Course
    [Authorize(Roles = "Student")]
    [HttpPost("{courseId}")]
    public async Task<IActionResult> Enroll(int courseId)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(studentId))
            return Unauthorized();

        await _enrollmentService.EnrollStudentAsync(studentId, courseId);

        return Ok("Enrolled successfully");
    }

    // Get My Courses
    [Authorize(Roles = "Student")]
    [HttpGet("my-courses")]
    public async Task<IActionResult> MyCourses()
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(studentId))
            return Unauthorized();

        var courses = await _enrollmentService
            .GetStudentEnrollmentsAsync(studentId);

        return Ok(courses);
    }

    // Update Progress
    [Authorize(Roles = "Student")]
    [HttpPut("progress")]
    public async Task<IActionResult> UpdateProgress(int courseId, int progress)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(studentId))
            return Unauthorized();

        await _enrollmentService
            .UpdateProgressAsync(studentId, courseId, progress);

        return Ok("Progress updated");
    }

    // Get Students In Course (Instructor)
    [Authorize(Roles = "Instructor")]
    [HttpGet("ByCourse/{courseId}")]
    public async Task<IActionResult> GetStudentsByCourse(int courseId)
    {
        var students = await _enrollmentService
            .GetStudentsByCourseAsync(courseId);

        return Ok(students);
    }
}




