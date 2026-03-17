using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlatformManagementSystem.Application.Interfaces;
using PlatformManagementSystem.Domain.Entities;
using System.ComponentModel.DataAnnotations;

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

    [HttpGet("ByCourse/{courseId}")]
    public async Task<IActionResult> GetByCourse(int courseId)
    {
        if (courseId <= 0)
            return BadRequest("Invalid course ID");

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
            Title = request.Title,
            Content = request.Content,
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
        if (id <= 0)
            return BadRequest("Invalid lesson ID");

        var lesson = await _unitOfWork.Lessons.GetByIdAsync(id);
        if (lesson == null)
            return NotFound();

        _unitOfWork.Lessons.Delete(lesson);
        await _unitOfWork.SaveChangesAsync();
        return Ok();
    }

    public sealed class CreateLessonRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "A valid CourseId is required")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(300, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 300 characters")]
        public string Title { get; set; } = "";

        [Required(ErrorMessage = "Content is required")]
        [StringLength(5000, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 5000 characters")]
        public string Content { get; set; } = "";

        public string? VideoUrl { get; set; }
    }
}
