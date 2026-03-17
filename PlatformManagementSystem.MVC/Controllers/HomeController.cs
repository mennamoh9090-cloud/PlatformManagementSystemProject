using Microsoft.AspNetCore.Mvc;
using PlatformManagementSystem.MVC.ViewModels.Course;
using System.Text.Json;

namespace PlatformManagementSystem.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("API");
            var response = await client.GetAsync("Course");
            var list = new List<CourseViewModel>();
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                list = JsonSerializer.Deserialize<List<CourseViewModel>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<CourseViewModel>();
            }
            return View(list);
        }
    }
}