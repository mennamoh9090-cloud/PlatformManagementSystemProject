using PlatformManagementSystem.Domain.Common;

namespace PlatformManagementSystem.Domain.Entities;

public class StudentExam : BaseEntity
{
    public int Id { get; set; }
    public string StudentId { get; set; } = null!;
    public ApplicationUser Student { get; set; } = null!;

    public int ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    public int Score { get; set; }
    public DateTime SubmittedAt { get; set; }
    public ICollection<StudentAnswer> StudentAnswers { get; set; }

}
