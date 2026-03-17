namespace PlatformManagementSystem.MVC.ViewModels.Live
{
    public class LiveSessionVM
    {
        public int Id { get; set; }
        public int CourseId { get; set; }

        public string CourseTitle { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string MeetingUrl { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public bool IsActive { get; set; }
    }
}
