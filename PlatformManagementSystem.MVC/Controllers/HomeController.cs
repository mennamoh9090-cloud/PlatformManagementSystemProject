using Microsoft.AspNetCore.Mvc;

namespace PlatformManagementSystem.MVC.Controllers
{
    
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}