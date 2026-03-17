using System.ComponentModel.DataAnnotations;

namespace PlatformManagementSystem.Application.DTOs.Course
{
    public class CreateCourseDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
        public string Title { get; set; } = "";

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters")]
        public string Description { get; set; } = "";

        [Range(0, 100000, ErrorMessage = "Price must be between 0 and 100,000")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A valid CategoryId is required")]
        public int CategoryId { get; set; }

        public string ThumbnailUrl { get; set; } = "";
    }
}
