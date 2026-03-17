using PlatformManagementSystem.Domain.Common;

namespace PlatformManagementSystem.Domain.Entities;

public class Answer : BaseEntity
{
    public string Text { get; set; } = null!;
    public bool IsCorrect { get; set; }

    public int QuestionId { get; set; }
    public Question Question { get; set; } = null!;
}

