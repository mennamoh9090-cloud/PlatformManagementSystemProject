namespace PlatformManagementSystem.MVC.ViewModels.InstructorDashboard
{
    public class CreateCourseVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public string InstructorId { get; set; } = string.Empty;
    }
}
