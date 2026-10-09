using Microsoft.AspNetCore.Mvc;

namespace PageApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
