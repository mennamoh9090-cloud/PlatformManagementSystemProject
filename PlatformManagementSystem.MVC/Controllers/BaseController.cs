using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace PlatformManagementSystem.MVC.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = HttpContext.Session.GetString("UserToken");

            if (string.IsNullOrEmpty(token))
            {
                context.Result = RedirectToAction("Login", "Account");
                return;
            }

            // Decode JWT
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var role = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

            ViewBag.UserRole = role;

            base.OnActionExecuting(context);
        }
    }
}
