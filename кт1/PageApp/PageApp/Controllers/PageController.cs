using Microsoft.AspNetCore.Mvc;

namespace PageApp.Controllers
{
    public class PageController : Controller
    {
        // Задание 1
        public IActionResult Welcome()
        {
            return View();
        }

        // Задание 2
        public IActionResult Greet(string name)
        {
            return View(model: name);
        }

        // Задание 3 — показ формы
        [HttpGet]
        public IActionResult Edit()
        {
            return View();
        }

        // Задание 3 — обработка формы
        [HttpPost]
        public IActionResult Edit(string message)
        {
            ViewBag.Message = message;
            return View();
        }
    }
}
