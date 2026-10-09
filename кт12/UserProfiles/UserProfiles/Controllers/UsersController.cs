using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserProfiles.Data;
using UserProfiles.Models;
using UserProfiles.ViewModels;

namespace UserProfiles.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContext _db;

        public UsersController(AppDbContext db)
        {
            _db = db;
        }

        // список пользователей вместе с профилями
        public async Task<IActionResult> Index()
        {
            var users = await _db.Users
                .Include(u => u.Profile)
                .OrderBy(u => u.Id)
                .ToListAsync();

            return View(users);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _db.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new UserFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserFormViewModel model)
        {
            // если профиль не создаем, его поля не проверяем
            if (!model.CreateProfile)
            {
                foreach (var key in ModelState.Keys.Where(k => k.StartsWith("Profile.")).ToList())
                    ModelState.Remove(key);
            }

            await CheckUniqueAsync(model);

            if (!ModelState.IsValid)
                return View(model);

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                CreatedAt = DateTime.Now
            };

            // пользователь и профиль сохраняются одним SaveChanges
            if (model.CreateProfile)
            {
                user.Profile = new UserProfile();
                FillProfile(user.Profile, model.Profile);
            }

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Пользователь {user.Username} создан";
            return RedirectToAction(nameof(Details), new { id = user.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            var model = new UserFormViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserFormViewModel model)
        {
            // на этой форме редактируется только сам пользователь
            foreach (var key in ModelState.Keys.Where(k => k.StartsWith("Profile.")).ToList())
                ModelState.Remove(key);

            var user = await _db.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            model.Id = id;
            await CheckUniqueAsync(model);

            if (!ModelState.IsValid)
                return View(model);

            user.Username = model.Username;
            user.Email = model.Email;
            await _db.SaveChangesAsync();

            TempData["Success"] = "Данные пользователя сохранены";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _db.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            // профиль отдельно не удаляем, его удалит каскад
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Пользователь {user.Username} удален вместе с профилем";
            return RedirectToAction(nameof(Index));
        }

        private async Task CheckUniqueAsync(UserFormViewModel model)
        {
            if (await _db.Users.AnyAsync(u => u.Username == model.Username && u.Id != model.Id))
                ModelState.AddModelError(nameof(model.Username), "Такой логин уже занят");

            if (await _db.Users.AnyAsync(u => u.Email == model.Email && u.Id != model.Id))
                ModelState.AddModelError(nameof(model.Email), "Такой email уже используется");
        }

        private static void FillProfile(UserProfile profile, ProfileFormViewModel data)
        {
            profile.FirstName = data.FirstName;
            profile.LastName = data.LastName;
            profile.BirthDate = data.BirthDate;
            profile.Phone = data.Phone;
            profile.City = string.IsNullOrWhiteSpace(data.City) ? "Не указан" : data.City;
            profile.Bio = data.Bio;
        }
    }
}
