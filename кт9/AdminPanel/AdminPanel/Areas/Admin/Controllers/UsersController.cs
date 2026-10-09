using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminPanel.Models;
using AdminPanel.ViewModels;

namespace AdminPanel.Areas.Admin.Controllers
{
    public class UsersController : AdminControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UsersController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // список пользователей
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.OrderBy(u => u.CreatedAt).ToListAsync();

            var model = new List<UserListItemViewModel>();
            foreach (var user in users)
            {
                model.Add(new UserListItemViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    FullName = user.FullName,
                    Roles = string.Join(", ", await _userManager.GetRolesAsync(user)),
                    CreatedAt = user.CreatedAt
                });
            }

            ViewBag.CurrentUserId = _userManager.GetUserId(User);
            return View(model);
        }

        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            ViewBag.Roles = string.Join(", ", await _userManager.GetRolesAsync(user));
            return View(user);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateUserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!Roles.All.Contains(model.Role))
                ModelState.AddModelError(nameof(model.Role), "Неизвестная роль");

            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return View(model);
            }

            await _userManager.AddToRoleAsync(user, model.Role);

            TempData["Success"] = $"Пользователь {user.Email} создан";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                Role = roles.FirstOrDefault() ?? Roles.User
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return NotFound();

            bool isCurrentUser = user.Id == _userManager.GetUserId(User);
            var currentRoles = await _userManager.GetRolesAsync(user);

            if (!Roles.All.Contains(model.Role))
                ModelState.AddModelError(nameof(model.Role), "Неизвестная роль");

            // чтобы админ случайно не забрал права у самого себя
            if (isCurrentUser && !currentRoles.Contains(model.Role))
                ModelState.AddModelError(nameof(model.Role), "Нельзя изменить роль самому себе");

            if (!ModelState.IsValid)
                return View(model);

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.UserName = model.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return View(model);
            }

            // смена роли
            if (!currentRoles.Contains(model.Role))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            // смена пароля, если указан новый
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                if (!passResult.Succeeded)
                {
                    AddErrors(passResult);
                    return View(model);
                }
            }

            // обновляем куки, если админ редактировал сам себя
            if (isCurrentUser)
                await _signInManager.RefreshSignInAsync(user);

            TempData["Success"] = $"Данные пользователя {user.Email} сохранены";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            if (user.Id == _userManager.GetUserId(User))
            {
                TempData["Error"] = "Нельзя удалить свою учетную запись";
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            if (user.Id == _userManager.GetUserId(User))
            {
                TempData["Error"] = "Нельзя удалить свою учетную запись";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                TempData["Success"] = $"Пользователь {user.Email} удален";
            else
                TempData["Error"] = "Не удалось удалить пользователя";

            return RedirectToAction(nameof(Index));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
    }
}
