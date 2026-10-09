using Microsoft.AspNetCore.Mvc;

namespace SecureChat.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
