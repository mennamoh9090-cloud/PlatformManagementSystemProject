using PlatformManagementSystem.Domain.Common;

namespace PlatformManagementSystem.Domain.Entities;

public class Question : BaseEntity
{
    public string Text { get; set; } = null!;

    public int ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();

    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}
