namespace PlatformManagementSystem.MVC.ViewModels.Course
{
    public class CourseViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string ThumbnailUrl {  get; set; }
        public string Description { get; set; }
        public string InstructorName {  get; set; }
        public bool IsEnrolled { get; set; }

        public decimal Price { get; set; }

        public double Rating { get; set; }
    }
}
