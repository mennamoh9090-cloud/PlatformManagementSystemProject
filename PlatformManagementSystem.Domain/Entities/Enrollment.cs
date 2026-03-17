using System;

namespace PlatformManagementSystem.Domain.Entities;

public class Enrollment
{
    public int Id { get; set; }

    public string StudentId { get; set; } = null!;
    public ApplicationUser Student { get; set; } = null!;

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public DateTime EnrollmentDate { get; set; }

    public int ProgressPercentage { get; set; }

    public bool IsCompleted { get; set; }
}
