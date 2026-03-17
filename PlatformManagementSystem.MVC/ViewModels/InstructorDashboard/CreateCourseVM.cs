namespace PlatformManagementSystem.MVC.ViewModels.InstructorDashboard;

public class CreateCourseVM
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public string? ThumbnailUrl { get; set; }
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Categories { get; set; } = [];
}
