using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlatformManagementSystem.Application.Interfaces;
using PlatformManagementSystem.Application.Interfaces.Services;
using PlatformManagementSystem.Domain.Entities;
using System.Security.Claims;

[Authorize(Roles = "Instructor,Student")]
[ApiController]
[Route("api/[controller]")]
public class ExamController : ControllerBase
{
    private readonly IExamService _examService;
    private readonly IUnitOfWork _unitOfWork;

    public ExamController(IExamService examService, IUnitOfWork unitOfWork)
    {
        _examService = examService;
        _unitOfWork = unitOfWork;
    }

    //  CREATE EXAM 
    [Authorize(Roles = "Instructor")]
    [HttpPost("create")]
    public async Task<IActionResult> Create(int courseId, string title, int duration)
    {
        var examId = await _examService.CreateExamAsync(courseId, title, duration);
        return Ok(examId);
    }

    [Authorize(Roles = "Instructor")]
    [HttpPost("CreateLegacy")]
    public async Task<IActionResult> CreateLegacy([FromBody] CreateExamRequest request)
    {
        var examId = await _examService.CreateExamAsync(
            request.CourseId,
            request.Title,
            request.DurationMinutes);

        return Ok(examId);
    }

    [Authorize(Roles = "Instructor")]
    [HttpPost("AddQuestion")]
    public async Task<IActionResult> AddQuestion([FromBody] AddQuestionRequest request)
    {
        var answers = (request.Answers ?? new List<AddQuestionAnswerRequest>())
            .Select(a => (a.Text ?? string.Empty, a.IsCorrect))
            .ToList();

        await _examService.AddQuestionAsync(request.ExamId, request.Text ?? string.Empty, answers);
        return Ok();
    }

    [Authorize(Roles = "Instructor,Student")]
    [HttpGet("{examId}")]
    public async Task<IActionResult> GetExamById(int examId)
    {
        var exam = await _unitOfWork.Exams.GetByIdAsync(examId);
        if (exam == null)
            return NotFound();

        var questions = await _unitOfWork.Questions.FindAsync(q => q.ExamId == examId);
        var questionIds = questions.Select(q => q.Id).ToList();
        var answers = (await _unitOfWork.Answers.GetAllAsync())
            .Where(a => questionIds.Contains(a.QuestionId))
            .ToList();

        return Ok(new
        {
            exam.Id,
            exam.Title,
            Questions = questions.Select(q => new
            {
                q.Id,
                q.Text,
                Answers = answers
                    .Where(a => a.QuestionId == q.Id)
                    .Select(a => new { a.Id, a.Text, a.IsCorrect })
            })
        });
    }

    // SUBMIT EXAM 
    [Authorize(Roles = "Student")]
    [HttpPost("{examId}/submit")]
    public async Task<IActionResult> Submit(int examId, [FromBody] Dictionary<int, int> answers)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(studentId))
            return Unauthorized();

        var score = await _examService.SubmitExamAsync(studentId, examId, answers);

        return Ok(new { Score = score });
    }

    [Authorize(Roles = "Student")]
    [HttpPost("Submit")]
    public async Task<IActionResult> SubmitLegacy([FromBody] SubmitExamRequest request)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(studentId))
            return Unauthorized();

        var score = await _examService.SubmitExamAsync(
            studentId,
            request.ExamId,
            request.SubmittedAnswers ?? new Dictionary<int, int>());

        return Ok(new { Score = score });
    }

    [Authorize(Roles = "Instructor")]
    [HttpGet("{examId}/Questions")]
    public async Task<IActionResult> GetQuestions(int examId)
    {
        var questions = await _unitOfWork.Questions.FindAsync(q => q.ExamId == examId);
        var questionIds = questions.Select(q => q.Id).ToList();
        var answers = (await _unitOfWork.Answers.GetAllAsync())
            .Where(a => questionIds.Contains(a.QuestionId))
            .ToList();

        return Ok(questions.Select(q => new
        {
            q.Id,
            q.ExamId,
            q.Text,
            Answers = answers
                .Where(a => a.QuestionId == q.Id)
                .Select(a => new { a.Id, a.Text, a.IsCorrect })
        }));
    }

    [Authorize(Roles = "Instructor")]
    [HttpGet("GetQuestion/{questionId}")]
    public async Task<IActionResult> GetQuestion(int questionId)
    {
        var question = await _unitOfWork.Questions.GetByIdAsync(questionId);
        if (question == null)
            return NotFound();

        var answers = await _unitOfWork.Answers.FindAsync(a => a.QuestionId == questionId);

        return Ok(new
        {
            questionId = question.Id,
            examId = question.ExamId,
            text = question.Text,
            answers = answers.OrderBy(a => a.Id).Select(a => a.Text).ToList()
        });
    }

    [Authorize(Roles = "Instructor")]
    [HttpPut("UpdateQuestion")]
    public async Task<IActionResult> UpdateQuestion([FromBody] UpdateQuestionRequest request)
    {
        var question = await _unitOfWork.Questions.GetByIdAsync(request.QuestionId);
        if (question == null)
            return NotFound();

        question.Text = request.Text ?? question.Text;
        _unitOfWork.Questions.Update(question);

        var currentAnswers = (await _unitOfWork.Answers.FindAsync(a => a.QuestionId == question.Id)).ToList();
        foreach (var answer in currentAnswers)
            _unitOfWork.Answers.Delete(answer);

        var answers = request.Answers ?? new List<AddQuestionAnswerRequest>();
        foreach (var answer in answers)
        {
            await _unitOfWork.Answers.AddAsync(new Answer
            {
                QuestionId = question.Id,
                Text = answer.Text ?? string.Empty,
                IsCorrect = answer.IsCorrect,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _unitOfWork.SaveChangesAsync();
        return Ok();
    }

    //  DELETE QUESTION
    [Authorize(Roles = "Instructor")]
    [HttpDelete("DeleteQuestion/{questionId}")]
    public async Task<IActionResult> DeleteQuestion(int questionId)
    {
        await _examService.DeleteQuestionAsync(questionId);
        return Ok();
    }

    // GET EXAMS BY COURSE 
    [Authorize(Roles = "Instructor")]
    [HttpGet("ByCourse/{courseId}")]
    public async Task<IActionResult> GetExamsByCourse(int courseId)
    {
        var exams = await _examService.GetExamsByCourseAsync(courseId);
        return Ok(exams);
    }

    public sealed class CreateExamRequest
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
    }

    public sealed class AddQuestionRequest
    {
        public int ExamId { get; set; }
        public string? Text { get; set; }
        public List<AddQuestionAnswerRequest>? Answers { get; set; }
    }

    public sealed class AddQuestionAnswerRequest
    {
        public string? Text { get; set; }
        public bool IsCorrect { get; set; }
    }

    public sealed class SubmitExamRequest
    {
        public int ExamId { get; set; }
        public Dictionary<int, int>? SubmittedAnswers { get; set; }
    }

    public sealed class UpdateQuestionRequest
    {
        public int QuestionId { get; set; }
        public string? Text { get; set; }
        public List<AddQuestionAnswerRequest>? Answers { get; set; }
    }
}

