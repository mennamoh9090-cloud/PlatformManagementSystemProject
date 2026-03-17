using PlatformManagementSystem.Domain.Enums;

public class InstructorCourseVM
{
    public int CourseId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int LessonsCount { get; set; }
    public int StudentCount { get; set; }
    public CourseStatus Status { get; set; }
}