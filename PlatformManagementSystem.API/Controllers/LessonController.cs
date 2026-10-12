using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlatformManagementSystem.Application.Interfaces;
using PlatformManagementSystem.Domain.Entities;

namespace PlatformManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public LessonController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetByCourse(int courseId)
    {
        var lessons = await _unitOfWork.Lessons.FindAsync(l => l.CourseId == courseId);
        var result = lessons
            .OrderBy(l => l.Id)
            .Select(l => new
            {
                l.Id,
                l.Title,
                l.Content
            });

        return Ok(result);
    }

    [Authorize(Roles = "Instructor")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLessonRequest request)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course == null)
            return NotFound("Course not found");

        var lesson = new Lesson
        {
            CourseId = request.CourseId,
            Title = request.Title ?? string.Empty,
            Content = request.Content ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Lessons.AddAsync(lesson);
        await _unitOfWork.SaveChangesAsync();
        return Ok(new { lesson.Id });
    }

    [Authorize(Roles = "Instructor")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var lesson = await _unitOfWork.Lessons.GetByIdAsync(id);
        if (lesson == null)
            return NotFound();

        _unitOfWork.Lessons.Delete(lesson);
        await _unitOfWork.SaveChangesAsync();
        return Ok();
    }

    public sealed class CreateLessonRequest
    {
        public int CourseId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
    }
}
