using PlatformManagementSystem.MVC.ViewModels.Course;

public class CourseDetailsViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ThumbnailUrl { get; set; }
    public decimal Price { get; set; }

    public string InstructorName { get; set; }
    public string InstructorBio { get; set; }

    public int StudentsCount { get; set; }
    public double Rating { get; set; }

    public bool IsEnrolled { get; set; }

    public List<LessonItemVM> Lessons { get; set; } = new();
    public List<ReviewItemVM> Reviews { get; set; } = new();
}

public class ReviewItemVM
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}