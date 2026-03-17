using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlatformManagementSystem.Application.DTOs.Dashboard.Admin;
using PlatformManagementSystem.Domain.Entities;
using PlatformManagementSystem.Infrastructure.Persistence;
using PlatformManagementSystem.Domain.Enums;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public AdminController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    // ROLES 

    [HttpGet("Roles")]
    public IActionResult GetRoles()
    {
        var roles = _roleManager.Roles
            .Select(r => new
            {
                r.Id,
                r.Name
            })
            .ToList();

        return Ok(roles);
    }

    [HttpPost("CreateRole")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto model)
    {
        if (string.IsNullOrWhiteSpace(model.RoleName))
            return BadRequest("Role name is required");

        if (await _roleManager.RoleExistsAsync(model.RoleName))
            return BadRequest("Role already exists");

        var result = await _roleManager.CreateAsync(new IdentityRole(model.RoleName));

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("Role created successfully");
    }

    [HttpDelete("DeleteRole/{roleName}")]
    public async Task<IActionResult> DeleteRole(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);

        if (role == null)
            return NotFound("Role not found");

        var result = await _roleManager.DeleteAsync(role);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("Role deleted");
    }

    [HttpPost("AssignRole")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);

        if (user == null)
            return NotFound("User not found");

        if (!await _roleManager.RoleExistsAsync(model.RoleName))
            return BadRequest("Role does not exist");

        var result = await _userManager.AddToRoleAsync(user, model.RoleName);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("Role assigned successfully");
    }

    [HttpPost("RemoveRole")]
    public async Task<IActionResult> RemoveRole([FromBody] AssignRoleDto model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);

        if (user == null)
            return NotFound("User not found");

        var result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("Role removed successfully");
    }

    //USERS 

    [HttpGet("Users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userManager.Users.ToListAsync();

        var result = new List<object>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            result.Add(new
            {
                user.Id,
                user.FullName,
                user.Email,
                Roles = roles
            });
        }

        return Ok(result);
    }

    [HttpDelete("DeleteUser/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound("User not found");

        await _userManager.DeleteAsync(user);
        return Ok("User deleted");
    }

    // COURSES

    [HttpGet("Courses")]
    public async Task<IActionResult> GetCourses()
    {
        var courses = await _context.Courses
            .Include(c => c.Enrollments)
            .Include(c => c.Instructor)
            .Select(c => new
            {
                c.Id,
                c.Title,
                StudentsCount = c.Enrollments.Count,
                Status = c.Status,
                InstructorName = c.Instructor.FullName
            })
            .ToListAsync();

        return Ok(courses);
    }

    [HttpDelete("DeleteCourse/{id}")]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null)
            return NotFound("Course not found");

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
        return Ok("Course deleted");
    }

    [HttpPost("ApproveCourse/{id}")]
    public async Task<IActionResult> ApproveCourse(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return NotFound("Course not found");

        course.Status = CourseStatus.Approved;
        await _context.SaveChangesAsync();
        return Ok("Course approved");
    }

    [HttpPost("RejectCourse/{id}")]
    public async Task<IActionResult> RejectCourse(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return NotFound("Course not found");

        course.Status = CourseStatus.Rejected;
        await _context.SaveChangesAsync();
        return Ok("Course rejected");
    }

    // STATS
    [HttpGet("PlatformStats")]
    public async Task<IActionResult> PlatformStats()
    {
        var totalUsers = await _context.Users.CountAsync();
        var totalCourses = await _context.Courses.CountAsync();
        var totalEnrollments = await _context.Enrollments.CountAsync();
        var totalExams = await _context.Exams.CountAsync(); 

        return Ok(new
        {
            totalUsers,
            totalCourses,
            totalEnrollments,
            totalExams
        });
    }
}