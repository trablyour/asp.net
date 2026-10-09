using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AdminPanel.Models;

namespace AdminPanel.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        // страница доступна любому вошедшему пользователю
        [Authorize(Policy = Policies.RegisteredUser)]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            ViewBag.Roles = string.Join(", ", await _userManager.GetRolesAsync(user));
            return View(user);
        }
    }
}
