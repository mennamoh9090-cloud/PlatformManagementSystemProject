using PlatformManagementSystem.Domain.Common;
using PlatformManagementSystem.Domain.Enums;

namespace PlatformManagementSystem.Domain.Entities;

public class Course : BaseEntity
{
    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public string ThumbnailUrl { get; set; } ="";
    public string InstructorId { get; set; } = null!;
    public ApplicationUser Instructor { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public CourseStatus Status { get; set; } = CourseStatus.Pending;

    public List<Lesson> Lessons { get; set; } = [];

    public List<Enrollment> Enrollments { get; set; } = [];

    public List<Review> Reviews { get; set; } = [];

    public List<Exam> Exams { get; set; } = [];   // 👈 علاقة واحدة بس

    public List<LiveSession> LiveSessions { get; set; } = [];
}
