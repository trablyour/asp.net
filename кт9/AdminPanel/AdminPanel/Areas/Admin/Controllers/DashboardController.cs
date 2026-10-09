using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminPanel.Models;

namespace AdminPanel.Areas.Admin.Controllers
{
    public class DashboardController : AdminControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.UsersCount = await _userManager.Users.CountAsync();
            ViewBag.AdminsCount = (await _userManager.GetUsersInRoleAsync(Roles.Admin)).Count;
            return View();
        }
    }
}
