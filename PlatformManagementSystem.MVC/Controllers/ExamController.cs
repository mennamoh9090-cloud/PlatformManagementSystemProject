using Microsoft.AspNetCore.Mvc;
using PlatformManagementSystem.MVC.ViewModels.Exam;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PlatformManagementSystem.MVC.Controllers
{
    public class ExamController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ExamController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // ================= CREATE EXAM =================

        [HttpGet]
        public IActionResult Create(int courseId)
        {
            return View(new CreateExamViewModel
            {
                CourseId = courseId
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateExamViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient("API");

            var token = HttpContext.Session.GetString("UserToken");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Exam", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Error creating exam");
                return View(model);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var examId = JsonSerializer.Deserialize<int>(responseContent);

            return RedirectToAction("AddQuestion", new { examId });
        }

        // ================= ADD QUESTION =================

        [HttpGet]
        public IActionResult AddQuestion(int examId)
        {
            return View(new AddQuestionViewModel
            {
                ExamId = examId,
                Answers = new List<string> { "", "", "", "" }
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddQuestion(AddQuestionViewModel model)
        {
            var client = _httpClientFactory.CreateClient("API");

            var token = HttpContext.Session.GetString("UserToken");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var answers = model.Answers
                .Select((a, index) => new
                {
                    text = a,
                    isCorrect = index == model.CorrectAnswerIndex
                }).ToList();

            var requestBody = new
            {
                examId = model.ExamId,
                text = model.Text,
                answers = answers
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Exam/AddQuestion", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Error adding question");
                return View(model);
            }

            return RedirectToAction("AddQuestion", new { examId = model.ExamId });
        }

        // ================= SOLVE EXAM =================

        [HttpGet]
        public async Task<IActionResult> Solve(int examId)
        {
            var client = _httpClientFactory.CreateClient("API");

            var token = HttpContext.Session.GetString("UserToken");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"Exam/{examId}");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index", "Courses");

            var json = await response.Content.ReadAsStringAsync();

            var exam = JsonSerializer.Deserialize<ExamDetailsViewModel>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return View(exam);
        }

        // ================= SUBMIT =================

        [HttpPost]
        public async Task<IActionResult> Submit(int examId, Dictionary<int, int> submittedAnswers)
        {
            var client = _httpClientFactory.CreateClient("API");

            var token = HttpContext.Session.GetString("UserToken");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var requestBody = new
            {
                examId,
                submittedAnswers
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Exam/Submit", content);

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index", "Courses");

            var score = await response.Content.ReadAsStringAsync();

            ViewBag.Score = score;

            return View("Result");
        }

        // ================= MANAGE QUESTIONS =================

        [HttpGet]
        public async Task<IActionResult> Questions(int examId)
        {
            var client = _httpClientFactory.CreateClient("API");

            var token = HttpContext.Session.GetString("UserToken");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"Exam/{examId}/Questions");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index", "Courses");

            var json = await response.Content.ReadAsStringAsync();

            var questions = JsonSerializer.Deserialize<List<QuestionViewModel>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            ViewBag.ExamId = examId;

            return View(questions);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteQuestion(int questionId, int examId)
        {
            var client = _httpClientFactory.CreateClient("API");
            var token = HttpContext.Session.GetString("UserToken");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"Exam/DeleteQuestion/{questionId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error deleting question";
            }

            return RedirectToAction("Questions", new { examId });
        }
        [HttpGet]
        public async Task<IActionResult> EditQuestion(int questionId)
        {
            var client = _httpClientFactory.CreateClient("API");

            var response = await client.GetAsync($"Exam/GetQuestion/{questionId}");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index", "Courses");

            var json = await response.Content.ReadAsStringAsync();

            var question = JsonSerializer.Deserialize<AddQuestionViewModel>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(question);
        }

        [HttpPost]
        public async Task<IActionResult> EditQuestion(AddQuestionViewModel model)
        {
            var client = _httpClientFactory.CreateClient("API");

            var answers = model.Answers
                .Select((a, index) => new
                {
                    text = a,
                    isCorrect = index == model.CorrectAnswerIndex
                }).ToList();

            var json = JsonSerializer.Serialize(new
            {
                questionId = model.QuestionId,
                text = model.Text,
                answers = answers
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync("Exam/UpdateQuestion", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error updating question";
            }

            return RedirectToAction("Questions", new { examId = model.ExamId });
        }
        public async Task<IActionResult> ViewExams(int courseId)
        {
            var token = HttpContext.Session.GetString("UserToken");

            var client = _httpClientFactory.CreateClient("API");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"Exam/ByCourse/{courseId}");

            var json = await response.Content.ReadAsStringAsync();

            var exams = JsonSerializer.Deserialize<List<ExamViewModel>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(exams);
        }


    }
}
