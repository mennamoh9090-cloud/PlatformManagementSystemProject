using Microsoft.AspNetCore.Mvc;
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : Controller
{
    [Route("Error")]
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
