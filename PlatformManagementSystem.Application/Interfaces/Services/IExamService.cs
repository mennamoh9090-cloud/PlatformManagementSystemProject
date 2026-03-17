using PlatformManagementSystem.Application.DTOs.Exam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformManagementSystem.Application.Interfaces.Services
{
    public interface IExamService
    {
        Task<int> CreateExamAsync(int courseId, string title, int duration);
        Task AddQuestionAsync(int examId, string text, List<(string text, bool isCorrect)> answers);
        Task<int> SubmitExamAsync(string studentId, int examId, Dictionary<int, int> answers);
        Task DeleteQuestionAsync(int questionId);
        Task<List<ExamDto>> GetExamsByCourseAsync(int courseId);


    }

}
