using System.ComponentModel.DataAnnotations;

namespace PlatformManagementSystem.MVC.ViewModels.Lesson
{
    public class CreateLessonViewModel
    {
        public int CourseId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? VideoUrl { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;
    }
}
