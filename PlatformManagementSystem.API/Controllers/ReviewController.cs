using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlatformManagementSystem.Application.DTOs;
using PlatformManagementSystem.Application.Interfaces;
using PlatformManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PlatformManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReviewController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    [HttpGet("Course/{courseId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourseReviews(int courseId)
    {
        var reviews = (await _unitOfWork.Reviews.FindAsync(r => r.CourseId == courseId)).ToList();
        var userIds = reviews.Select(r => r.StudentId).Distinct().ToList();
        var users = (await _unitOfWork.Users.FindAsync(u => userIds.Contains(u.Id))).ToDictionary(u => u.Id, u => u.FullName ?? u.UserName ?? "Student");

        var reviewDtos = reviews.Select(r => new ReviewDto
        {
            Id = r.Id,
            CourseId = r.CourseId,
            StudentId = r.StudentId,
            StudentName = users.GetValueOrDefault(r.StudentId, "Student"),
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        }).OrderByDescending(r => r.CreatedAt).ToList();

        var averageRating = reviewDtos.Any() ? Math.Round(reviewDtos.Average(r => r.Rating), 1) : 0;

        return Ok(new
        {
            CourseId = courseId,
            AverageRating = averageRating,
            TotalReviews = reviewDtos.Count,
            Reviews = reviewDtos
        });
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> AddOrUpdateReview([FromBody] CreateReviewDto dto)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(studentId))
            return Unauthorized();

        if (dto.Rating < 1 || dto.Rating > 5)
            return BadRequest("Rating must be between 1 and 5 stars.");

        if (string.IsNullOrWhiteSpace(dto.Comment))
            return BadRequest("Review comment cannot be empty.");

        var enrollment = await _unitOfWork.Enrollments
            .FirstOrDefaultAsync(e => e.CourseId == dto.CourseId && e.StudentId == studentId);

        if (enrollment == null)
            return BadRequest("You must be enrolled in this course to leave a review.");

        var existing = await _unitOfWork.Reviews
            .FirstOrDefaultAsync(r => r.CourseId == dto.CourseId && r.StudentId == studentId);

        if (existing != null)
        {
            existing.Rating = dto.Rating;
            existing.Comment = dto.Comment;
            _unitOfWork.Reviews.Update(existing);
        }
        else
        {
            var student = await _userManager.FindByIdAsync(studentId);
            var course = await _unitOfWork.Courses.GetByIdAsync(dto.CourseId);
            if (student == null || course == null)
                return NotFound("Student or Course not found.");

            var review = new Review
            {
                CourseId = dto.CourseId,
                Course = course,
                StudentId = studentId,
                Student = student,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            await _unitOfWork.Reviews.AddAsync(review);
        }

        await _unitOfWork.SaveChangesAsync();
        return Ok(new { message = "Review saved successfully." });
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review == null)
            return NotFound("Review not found.");

        if (!isAdmin && review.StudentId != userId)
            return Forbid();

        _unitOfWork.Reviews.Remove(review);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { message = "Review deleted successfully." });
    }
}
