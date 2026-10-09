using Microsoft.AspNetCore.Mvc;

namespace ResponseApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
