using PlatformManagementSystem.Domain.Common;

namespace PlatformManagementSystem.Domain.Entities;

public class StudentAnswer : BaseEntity
{
    public int StudentExamId { get; set; }
    public StudentExam StudentExam { get; set; } = null!;

    public int QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public int SelectedAnswerId { get; set; }
}
