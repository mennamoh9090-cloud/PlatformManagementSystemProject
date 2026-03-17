using System.ComponentModel.DataAnnotations;

namespace PlatformManagementSystem.MVC.ViewModels.Exam
{
    public class AddQuestionViewModel
    {
        public int ExamId { get; set; }

        [Required]
        public string Text { get; set; }

        public required List<string> Answers { get; set; }

        public int CorrectAnswerIndex { get; set; }
        public object? QuestionId { get; internal set; }
    }
}
