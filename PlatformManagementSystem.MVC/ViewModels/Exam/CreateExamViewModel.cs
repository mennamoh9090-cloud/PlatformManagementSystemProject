using System.ComponentModel.DataAnnotations;

namespace PlatformManagementSystem.MVC.ViewModels.Exam
{
    public class CreateExamViewModel
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public int DurationMinutes { get; set; }
    }
}
