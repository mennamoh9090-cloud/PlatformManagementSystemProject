namespace PlatformManagementSystem.MVC.ViewModels.Student
{
    public class CompletionVM
    {
        public int Completed { get; set; }
        public int TotalCourses { get; set; }
        public double CompletionPercentage => (double)Completed/ TotalCourses * 100;
    }

}
