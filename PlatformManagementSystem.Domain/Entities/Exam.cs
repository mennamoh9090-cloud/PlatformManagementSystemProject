using PlatformManagementSystem.Domain.Common;

namespace PlatformManagementSystem.Domain.Entities;

public class Exam : BaseEntity
{
    public string Title { get; set; }

    public int CourseId { get; set; }   // FK
    public Course Course { get; set; }  // Navigation Property

    public int DurationMinutes { get; set; }

    public List<Question> Questions { get; set; } = new();

    public List<StudentExam> StudentExams { get; set; } = new();
}