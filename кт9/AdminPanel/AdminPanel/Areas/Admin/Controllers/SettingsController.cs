using Microsoft.AspNetCore.Mvc;
using AdminPanel.Services;
using AdminPanel.ViewModels;

namespace AdminPanel.Areas.Admin.Controllers
{
    public class SettingsController : AdminControllerBase
    {
        private readonly SiteSettings _settings;

        public SettingsController(SiteSettings settings)
        {
            _settings = settings;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new SettingsViewModel
            {
                SiteName = _settings.SiteName,
                AllowRegistration = _settings.AllowRegistration
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(SettingsViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _settings.SiteName = model.SiteName;
            _settings.AllowRegistration = model.AllowRegistration;

            TempData["Success"] = "Настройки сохранены";
            return RedirectToAction(nameof(Index));
        }
    }
}
