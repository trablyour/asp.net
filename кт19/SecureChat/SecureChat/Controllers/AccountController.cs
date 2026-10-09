using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureChat.Data;
using SecureChat.Models;
using SecureChat.ViewModels;

namespace SecureChat.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        private readonly PasswordHasher<User> _hasher = new PasswordHasher<User>();
        private readonly ILogger<AccountController> _logger;

        public AccountController(AppDbContext db, ILogger<AccountController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // ---------- регистрация ----------

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Chat");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid && await _db.Users.AnyAsync(u => u.Username == model.Username))
                ModelState.AddModelError(nameof(model.Username), "Этот логин уже занят");

            if (!ModelState.IsValid)
                return View(model);

            var user = new User
            {
                Username = model.Username,
                CreatedAt = DateTime.UtcNow
            };
            user.PasswordHash = _hasher.HashPassword(user, model.Password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Зарегистрирован пользователь {User}", user.Username);

            // после регистрации сразу входим
            await SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Chat");
        }

        // ---------- вход ----------

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Chat");

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

            // одно и то же сообщение, чтобы нельзя было узнать, существует ли логин
            if (user == null ||
                _hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Неверный логин или пароль");
                return View(model);
            }

            await SignInAsync(user, model.RememberMe);
            _logger.LogInformation("Вход пользователя {User}", user.Username);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Chat");
        }

        // ---------- выход ----------

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        // создаем куку с данными пользователя (claims)
        private async Task SignInAsync(User user, bool isPersistent)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var properties = new AuthenticationProperties
            {
                // "Запомнить меня": кука переживет закрытие браузера
                IsPersistent = isPersistent,
                ExpiresUtc = isPersistent ? DateTimeOffset.UtcNow.AddDays(7) : null
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                properties);
        }
    }
}
