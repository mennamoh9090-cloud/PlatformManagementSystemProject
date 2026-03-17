using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PlatformManagementSystem.MVC.ViewModels.Lesson;
using System.Text;

namespace PlatformManagementSystem.MVC.Controllers
{
    public class LessonsController : Controller
    {
        private readonly HttpClient _httpClient;

        public LessonsController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7102/api/");
        }

        // عرض دروس كورس معين
        public async Task<IActionResult> Index(int courseId)
        {
            var token = HttpContext.Session.GetString("UserToken");

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"Lesson/course/{courseId}");

            if (!response.IsSuccessStatusCode)
                return View(new List<LessonViewModel>());

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<List<LessonViewModel>>(json);

            ViewBag.CourseId = courseId;

            return View(data);
        }

        // GET: Create
        public IActionResult Create(int courseId)
        {
            return View(new CreateLessonViewModel { CourseId = courseId });
        }

        // POST: Create
        [HttpPost]
        public async Task<IActionResult> Create(CreateLessonViewModel model)
        {
            var token = HttpContext.Session.GetString("UserToken");

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(
                JsonConvert.SerializeObject(model),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("Lesson", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("ManageCourse", "InstructorDashboard", new { courseId = model.CourseId });

            return View(model);
        }

        public async Task<IActionResult> Delete(int id, int courseId)
        {
            var token = HttpContext.Session.GetString("UserToken");

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            await _httpClient.DeleteAsync($"Lesson/{id}");

            return RedirectToAction("Index", new { courseId });
        }
    }
}


