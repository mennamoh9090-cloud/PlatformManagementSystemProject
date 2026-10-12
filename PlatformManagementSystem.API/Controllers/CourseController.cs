using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlatformManagementSystem.Application.DTOs.Course;
using PlatformManagementSystem.Application.Interfaces.Services;
using PlatformManagementSystem.Domain.Entities;
using PlatformManagementSystem.Infrastructure.Persistence;
using System.Security.Claims;

namespace PlatformManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICourseService _courseService;

        public CourseController(
            ApplicationDbContext context,
            ICourseService courseService)
        {
            _context = context;
            _courseService = courseService;
        }

        
        //  Get All Courses
        
        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _context.Courses
                .Select(c => new
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description
                })
                .ToListAsync();

            return Ok(courses);
        }

        //  Create Course
        [Authorize(Roles = "Instructor")]
        [HttpPost]
        public async Task<IActionResult> CreateCourse(CreateCourseDto dto)
        {
            var instructorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(instructorId))
                return Unauthorized();

            var course = new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                InstructorId = instructorId
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Course Created Successfully" });
        }

        //  Get Courses By Instructor
        [Authorize(Roles = "Instructor")]
        [HttpGet("ByInstructor/{instructorId}")]
        public async Task<IActionResult> GetCoursesByInstructor(string instructorId)
        {
            var courses = await _context.Courses
                .Where(c => c.InstructorId == instructorId)
                .Include(c => c.Lessons)
                .Include(c => c.Enrollments)
                .Select(c => new
                {
                    CourseId = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    LessonsCount = c.Lessons.Count,
                    StudentsCount = c.Enrollments.Count
                })
                .ToListAsync();

            return Ok(courses);
        }

        //  Instructor Stats
        [Authorize(Roles = "Instructor")]
        [HttpGet("InstructorStats/{instructorId}")]
        public async Task<IActionResult> GetInstructorStats(string instructorId)
        {
            var courses = await _context.Courses
                .Where(c => c.InstructorId == instructorId)
                .Include(c => c.Enrollments)
                .Include(c => c.Exams)
                .ToListAsync();

            var totalCourses = courses.Count;

            var totalStudents = courses
                .SelectMany(c => c.Enrollments)
                .Select(e => e.StudentId)
                .Distinct()
                .Count();

            var totalExams = courses
                .SelectMany(c => c.Exams)
                .Count();

            return Ok(new
            {
                totalCourses,
                totalStudents,
                totalExams
            });
        }

        //  Course Details
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetCourseDetails(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Lessons)
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound();

            var reviews = await _context.Reviews
                .Include(r => r.Student)
                .Where(r => r.CourseId == id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isEnrolled = !string.IsNullOrEmpty(currentUserId) &&
                             course.Enrollments.Any(e => e.StudentId == currentUserId);

            var avgRating = reviews.Any() ? Math.Round(reviews.Average(r => r.Rating), 1) : 0.0;

            return Ok(new
            {
                course.Id,
                course.Title,
                course.Description,
                course.ThumbnailUrl,
                course.Price,
                InstructorName = course.Instructor?.FullName ?? "Instructor",
                InstructorBio = "Expert Instructor",
                StudentsCount = course.Enrollments.Count,
                Rating = avgRating,
                IsEnrolled = isEnrolled,
                Reviews = reviews.Select(r => new
                {
                    r.Id,
                    r.Rating,
                    r.Comment,
                    StudentName = r.Student?.FullName ?? r.Student?.UserName ?? "Student",
                    r.CreatedAt
                }),
                Lessons = course.Lessons.Select(l => new
                {
                    l.Id,
                    l.Title
                })
            });
        }

        // Update Course
        [Authorize(Roles = "Instructor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CreateCourseDto dto)
        {
            var instructorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(instructorId))
                return Unauthorized();

            try
            {
                await _courseService.UpdateAsync(id, dto, instructorId);
                return Ok(new { message = "Course updated successfully" });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Delete Course
        [Authorize(Roles = "Instructor,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            try
            {
                await _courseService.DeleteAsync(id);
                return Ok(new { message = "Course deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Approve Course (Admin)
        [Authorize(Roles = "Admin")]
        [HttpPut("Approve/{id}")]
        public async Task<IActionResult> ApproveCourse(int id)
        {
            try
            {
                await _courseService.ApproveCourseAsync(id);
                return Ok(new { message = "Course approved successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Reject Course (Admin)
        [Authorize(Roles = "Admin")]
        [HttpPut("Reject/{id}")]
        public async Task<IActionResult> RejectCourse(int id)
        {
            try
            {
                await _courseService.RejectCourseAsync(id);
                return Ok(new { message = "Course rejected successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}