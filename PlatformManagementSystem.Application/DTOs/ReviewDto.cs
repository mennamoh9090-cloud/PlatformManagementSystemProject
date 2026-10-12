using System;

namespace PlatformManagementSystem.Application.DTOs;

public class ReviewDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateReviewDto
{
    public int CourseId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}
