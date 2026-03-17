using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlatformManagementSystem.Application.DTOs.Course;
using PlatformManagementSystem.Application.Interfaces.Services;
using PlatformManagementSystem.Domain.Entities;
using PlatformManagementSystem.Infrastructure.Persistence;
using System.Security.Claims;
using PlatformManagementSystem.Domain.Enums;

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
                .Where(c => c.Status == CourseStatus.Approved)
                .Include(c => c.Instructor)
                .Select(c => new
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    ThumbnailUrl = c.ThumbnailUrl ?? "",
                    InstructorName = c.Instructor != null ? c.Instructor.FullName : "",
                    Price = c.Price
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
                InstructorId = instructorId,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                ThumbnailUrl = dto.ThumbnailUrl,
                Status = CourseStatus.Approved
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
            if (string.IsNullOrWhiteSpace(instructorId))
                return BadRequest("Instructor ID is required");

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
                    StudentCount = c.Enrollments.Count,
                    Status = c.Status
                })
                .ToListAsync();

            return Ok(courses);
        }

        //  Instructor Stats
        [Authorize(Roles = "Instructor")]
        [HttpGet("InstructorStats/{instructorId}")]
        public async Task<IActionResult> GetInstructorStats(string instructorId)
        {
            if (string.IsNullOrWhiteSpace(instructorId))
                return BadRequest("Instructor ID is required");

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
            if (id <= 0)
                return BadRequest("Invalid course ID");

            var course = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Lessons)
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == id && c.Status == CourseStatus.Approved);

            if (course == null)
                return NotFound();

            return Ok(new
            {
                course.Id,
                course.Title,
                course.Description,
                course.Price,
                ThumbnailUrl = course.ThumbnailUrl ?? "",
                InstructorName = course.Instructor.FullName,
                StudentsCount = course.Enrollments.Count,
                Lessons = course.Lessons.Select(l => new
                {
                    l.Id,
                    l.Title
                })
            });
        }

        //  Instructor Course Details
        [Authorize(Roles = "Instructor")]
        [HttpGet("InstructorDetails/{id}")]
        public async Task<IActionResult> GetInstructorCourseDetails(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid course ID");

            var instructorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(instructorId))
                return Unauthorized();

            var course = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Lessons)
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == id && c.InstructorId == instructorId);

            if (course == null)
                return NotFound();

            return Ok(new
            {
                course.Id,
                course.Title,
                course.Description,
                course.Price,
                ThumbnailUrl = course.ThumbnailUrl ?? "",
                InstructorName = course.Instructor.FullName,
                StudentsCount = course.Enrollments.Count,
                Lessons = course.Lessons.Select(l => new
                {
                    l.Id,
                    l.Title
                })
            });
        }
    }
}