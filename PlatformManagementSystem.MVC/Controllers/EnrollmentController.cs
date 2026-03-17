using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlatformManagementSystem.Domain.Entities;
using PlatformManagementSystem.Infrastructure.Persistence;

[Authorize(Roles = "Student")]
public class EnrollmentsController(ApplicationDbContext context,
                             UserManager<ApplicationUser> userManager) : Controller
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    [HttpPost]
    public async Task<IActionResult> Enroll(int courseId)
    {
        var user = await _userManager.GetUserAsync(User);

        var exists = _context.Enrollments
            .Any(e => e.CourseId == courseId && e.StudentId == user.Id);

        if (exists)
        {
            TempData["Error"] = "You already enrolled in this course.";
            return RedirectToAction("Index", "Courses");
        }

        var enrollment = new Enrollment
        {
            CourseId = courseId,
            StudentId = user.Id
        };

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Enrollment successful!";
        return RedirectToAction("Index", "Courses");
    }
}

