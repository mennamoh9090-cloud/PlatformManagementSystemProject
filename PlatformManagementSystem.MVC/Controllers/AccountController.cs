using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PlatformManagementSystem.MVC.ViewModels.Auth;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace PlatformManagementSystem.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // ================= LOGIN GET =================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ================= LOGIN POST =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient("API");

            var content = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("Auth/login", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Invalid Email or Password");
                return View(model);
            }

            var responseString = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<LoginResponseVM>(
                responseString,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (result == null || string.IsNullOrEmpty(result.Token))
            {
                ModelState.AddModelError("", "Login Failed");
                return View(model);
            }

            // ✅ خزّن التوكن
            HttpContext.Session.SetString("UserToken", result.Token);
            HttpContext.Session.SetString("UserId",result.UserId);
            HttpContext.Session.SetString("UserRole", result.Role);


            // ✅ ضيف UserId في الـ Claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, result.UserId),  // 👈 أهم سطر
        new Claim(ClaimTypes.Name, result.Email),
        new Claim(ClaimTypes.Role, result.Role)
    };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            if (result.Role == "Student")
                return RedirectToAction("Index", "StudentDashboard");

            if (result.Role == "Admin")
                return RedirectToAction("Index", "AdminDashboard");

            return RedirectToAction("Index", "InstructorDashboard");
        }

        // ================= REGISTER GET =================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // ================= REGISTER POST =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient("API");

            var content = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("Auth/register", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Login");

            ModelState.AddModelError("", "Registration Failed");
            return View(model);
        }

        // ================= LOGOUT =================
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}