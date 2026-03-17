using Microsoft.EntityFrameworkCore;
using PlatformManagementSystem.Application.DTOs.Exam;
using PlatformManagementSystem.Application.Interfaces;
using PlatformManagementSystem.Application.Interfaces.Services;
using PlatformManagementSystem.Domain.Entities;

namespace PlatformManagementSystem.Application.Services
{
    public class ExamService : IExamService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExamService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> CreateExamAsync(int courseId, string title, int duration)
        {
            var exam = new Exam
            {
                CourseId = courseId,
                Title = title,
                DurationMinutes = duration,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Exams.AddAsync(exam);
            await _unitOfWork.SaveChangesAsync();

            return exam.Id;
        }

        
        public async Task AddQuestionAsync(
            int examId,
            string text,
            List<(string text, bool isCorrect)> answers)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(examId);
            if (exam == null)
                throw new Exception("Exam not found.");

            var question = new Question
            {
                ExamId = examId,
                Text = text,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Questions.AddAsync(question);
            await _unitOfWork.SaveChangesAsync();

            foreach (var ans in answers)
            {
                await _unitOfWork.Answers.AddAsync(new Answer
                {
                    QuestionId = question.Id,
                    Text = ans.text,
                    IsCorrect = ans.isCorrect,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _unitOfWork.SaveChangesAsync();
        }

        
        public async Task<int> SubmitExamAsync(
            string studentId,
            int examId,
            Dictionary<int, int> submittedAnswers)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(examId);
            if (exam == null)
                throw new Exception("Exam not found.");

            var enrollment = await _unitOfWork.Enrollments
                .FirstOrDefaultAsync(e =>
                    e.StudentId == studentId &&
                    e.CourseId == exam.CourseId);

            if (enrollment == null)
                throw new Exception("You are not enrolled in this course.");

            var alreadySubmitted = await _unitOfWork.StudentExams
                .FirstOrDefaultAsync(se =>
                    se.StudentId == studentId &&
                    se.ExamId == examId);

            if (alreadySubmitted != null)
                throw new Exception("You already submitted this exam.");

            var questions = await _unitOfWork.Questions
                .FindAsync(q => q.ExamId == examId);

            int score = 0;

            foreach (var question in questions)
            {
                if (submittedAnswers.TryGetValue(question.Id, out int selectedAnswerId))
                {
                    var answer = await _unitOfWork.Answers
                        .FirstOrDefaultAsync(a =>
                            a.Id == selectedAnswerId &&
                            a.QuestionId == question.Id);

                    if (answer != null && answer.IsCorrect)
                        score++;
                }
            }

            var studentExam = new StudentExam
            {
                StudentId = studentId,
                ExamId = examId,
                Score = score,
                SubmittedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.StudentExams.AddAsync(studentExam);
            await _unitOfWork.SaveChangesAsync();

            await UpdateStudentProgress(studentId, exam.CourseId);

            return score;
        }

        private async Task UpdateStudentProgress(string studentId, int courseId)
        {
            var progresses = await _unitOfWork.StudentLessonProgresses
                .FindAsync(p =>
                    p.StudentId == studentId &&
                    p.CourseId == courseId);

            var progress = progresses.FirstOrDefault();

            if (progress != null)
            {
                progress.IsCompleted = true;
                progress.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.StudentLessonProgresses.Update(progress);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteQuestionAsync(int questionId)
        {
            var question = await _unitOfWork.Questions.GetByIdAsync(questionId);
            if (question == null)
                throw new Exception("Question not found.");

            var answers = await _unitOfWork.Answers.FindAsync(a => a.QuestionId == questionId);
            foreach (var answer in answers)
                _unitOfWork.Answers.Delete(answer);

            _unitOfWork.Questions.Delete(question);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<ExamDto>> GetExamsByCourseAsync(int courseId)
        {
            var exams = await _unitOfWork.Exams
                .FindAsync(e => e.CourseId == courseId);

            return exams
                .Select(e => new ExamDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Duration = e.DurationMinutes,
                    CourseId = e.CourseId
                })
                .ToList();
        }


    }
}

