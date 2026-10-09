using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminPanel.Models;
using AdminPanel.ViewModels;

namespace AdminPanel.Areas.Admin.Controllers
{
    public class ReportsController : AdminControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportsController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var weekAgo = DateTime.Now.AddDays(-7);

            var model = new ReportsViewModel
            {
                TotalUsers = users.Count,
                AdminCount = (await _userManager.GetUsersInRoleAsync(Roles.Admin)).Count,
                UserCount = (await _userManager.GetUsersInRoleAsync(Roles.User)).Count,
                NewLast7Days = users.Count(u => u.CreatedAt >= weekAgo)
            };

            foreach (var user in users.OrderByDescending(u => u.CreatedAt).Take(5))
            {
                model.LatestUsers.Add(new UserListItemViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    FullName = user.FullName,
                    Roles = string.Join(", ", await _userManager.GetRolesAsync(user)),
                    CreatedAt = user.CreatedAt
                });
            }

            return View(model);
        }
    }
}
