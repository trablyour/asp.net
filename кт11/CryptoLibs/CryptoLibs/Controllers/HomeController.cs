using Microsoft.AspNetCore.Mvc;

namespace CryptoLibs.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
