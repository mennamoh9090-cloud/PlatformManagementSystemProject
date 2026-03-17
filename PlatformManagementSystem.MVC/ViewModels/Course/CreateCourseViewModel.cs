using System.ComponentModel.DataAnnotations;

namespace PlatformManagementSystem.MVC.ViewModels.Course
{
    public class CreateCourseViewModel
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
    }
}
