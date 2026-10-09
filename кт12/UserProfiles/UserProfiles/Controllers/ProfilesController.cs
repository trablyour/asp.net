using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserProfiles.Data;
using UserProfiles.Models;
using UserProfiles.ViewModels;

namespace UserProfiles.Controllers
{
    // профиль всегда ищем по id пользователя: у каждого пользователя он один
    public class ProfilesController : Controller
    {
        private readonly AppDbContext _db;

        public ProfilesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int userId)
        {
            var user = await _db.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return NotFound();

            if (user.Profile != null)
            {
                TempData["Error"] = "У пользователя уже есть профиль";
                return RedirectToAction("Details", "Users", new { id = userId });
            }

            return View(new ProfileFormViewModel { UserId = userId, Username = user.Username });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProfileFormViewModel model)
        {
            var user = await _db.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.Id == model.UserId);
            if (user == null)
                return NotFound();

            if (user.Profile != null)
            {
                TempData["Error"] = "У пользователя уже есть профиль";
                return RedirectToAction("Details", "Users", new { id = model.UserId });
            }

            if (!ModelState.IsValid)
            {
                model.Username = user.Username;
                return View(model);
            }

            var profile = new UserProfile { UserId = user.Id };
            Fill(profile, model);

            _db.UserProfiles.Add(profile);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Профиль создан";
            return RedirectToAction("Details", "Users", new { id = user.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int userId)
        {
            var profile = await _db.UserProfiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
                return NotFound();

            var model = new ProfileFormViewModel
            {
                UserId = profile.UserId,
                Username = profile.User.Username,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                BirthDate = profile.BirthDate,
                Phone = profile.Phone,
                City = profile.City,
                Bio = profile.Bio
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileFormViewModel model)
        {
            var profile = await _db.UserProfiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == model.UserId);

            if (profile == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                model.Username = profile.User.Username;
                return View(model);
            }

            Fill(profile, model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Профиль сохранен";
            return RedirectToAction("Details", "Users", new { id = model.UserId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int userId)
        {
            var profile = await _db.UserProfiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
                return NotFound();

            return View(profile);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int userId)
        {
            var profile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null)
                return NotFound();

            // удаляется только профиль, пользователь остается
            _db.UserProfiles.Remove(profile);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Профиль удален, пользователь остался";
            return RedirectToAction("Details", "Users", new { id = userId });
        }

        private static void Fill(UserProfile profile, ProfileFormViewModel data)
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
