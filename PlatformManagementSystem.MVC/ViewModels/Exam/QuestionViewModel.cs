namespace PlatformManagementSystem.MVC.ViewModels.Exam
{
    public class QuestionViewModel
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public string QuestionText { get; set; }

        public string Text { get; set; }

        public required List<AnswerViewModel> Answers { get; set; }
    }
}
