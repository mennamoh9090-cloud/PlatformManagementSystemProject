namespace PlatformManagementSystem.MVC.ViewModels.Exam
{
    public class ExamDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<QuestionViewModel> Questions { get; set; }
    }
}
