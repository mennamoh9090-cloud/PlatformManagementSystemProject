namespace PlatformManagementSystem.Application.DTOs.Exam
{
    public class ExamDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = "";

        public int Duration { get; set; }

        public int CourseId { get; set; }
    }
}
