using Microsoft.AspNetCore.Mvc;

namespace PlatformManagementSystem.MVC.Controllers
{
    public class DashboardController : BaseController
    {
        public IActionResult Admin()
        {
            if (ViewBag.UserRole != "Admin")
                return RedirectToAction("Login", "Account");

            return View();
        }

        public IActionResult Instructor()
        {
            if (ViewBag.UserRole != "Instructor")
                return RedirectToAction("Login", "Account");

            return View();
        }

        public IActionResult Student()
        {
            if (ViewBag.UserRole != "Student")
                return RedirectToAction("Login", "Account");

            return View();
        }
    }
}

