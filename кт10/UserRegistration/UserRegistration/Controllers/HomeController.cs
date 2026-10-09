using Microsoft.AspNetCore.Mvc;
using UserRegistration.Services;
using UserRegistration.ViewModels;

namespace UserRegistration.Controllers
{
    // Задание 1. Форма регистрации
    public class HomeController : Controller
    {
        private readonly UserService _users;

        public HomeController(UserService users)
        {
            _users = users;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RegisterViewModel model)
        {
            // проверка уникальности, только если сами поля прошли валидацию
            if (ModelState.IsValid)
            {
                if (await _users.UsernameTakenAsync(model.Username))
                    ModelState.AddModelError(nameof(model.Username), "Пользователь с таким именем уже существует");

                if (await _users.EmailTakenAsync(model.Email))
                    ModelState.AddModelError(nameof(model.Email), "Этот email уже зарегистрирован");
            }

            if (!ModelState.IsValid)
                return View(model);

            await _users.CreateAsync(model.Username, model.Email, model.Password);

            TempData["Success"] = $"Пользователь {model.Username} успешно зарегистрирован!";
            return RedirectToAction(nameof(Index));
        }

        // список зарегистрированных, чтобы видеть, что запись попала в базу
        public async Task<IActionResult> Users()
        {
            return View(await _users.GetAllAsync());
        }
    }
}
