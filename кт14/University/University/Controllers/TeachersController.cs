using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University.Data;
using University.Models;
using University.ViewModels;

namespace University.Controllers
{
    public class TeachersController : Controller
    {
        private readonly UniversityContext _db;

        public TeachersController(UniversityContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var teachers = await _db.Teachers
                .Include(t => t.Courses)
                .OrderBy(t => t.LastName)
                .ToListAsync();

            return View(teachers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var teacher = await _db.Teachers
                .Include(t => t.Courses)
                    .ThenInclude(c => c.Enrollments)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (teacher == null)
                return NotFound();

            return View(teacher);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new TeacherFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeacherFormViewModel model)
        {
            await CheckEmailAsync(model);
            if (!ModelState.IsValid)
                return View(model);

            var teacher = new Teacher();
            Fill(teacher, model);

            _db.Teachers.Add(teacher);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Преподаватель {teacher.FullName} добавлен";
            return RedirectToAction(nameof(Details), new { id = teacher.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var teacher = await _db.Teachers.FindAsync(id);
            if (teacher == null)
                return NotFound();

            return View(new TeacherFormViewModel
            {
                Id = teacher.Id,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                Email = teacher.Email,
                Department = teacher.Department
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TeacherFormViewModel model)
        {
            var teacher = await _db.Teachers.FindAsync(id);
            if (teacher == null)
                return NotFound();

            model.Id = id;
            await CheckEmailAsync(model);
            if (!ModelState.IsValid)
                return View(model);

            Fill(teacher, model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Данные преподавателя сохранены";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var teacher = await _db.Teachers
                .Include(t => t.Courses)
                    .ThenInclude(c => c.Enrollments)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (teacher == null)
                return NotFound();

            return View(teacher);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _db.Teachers.FindAsync(id);
            if (teacher == null)
                return NotFound();

            // курсы и записи на них удалит каскад
            _db.Teachers.Remove(teacher);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Преподаватель {teacher.FullName} удален вместе со своими курсами";
            return RedirectToAction(nameof(Index));
        }

        private async Task CheckEmailAsync(TeacherFormViewModel model)
        {
            if (await _db.Teachers.AnyAsync(t => t.Email == model.Email && t.Id != model.Id))
                ModelState.AddModelError(nameof(model.Email), "Этот email уже используется");
        }

        private static void Fill(Teacher teacher, TeacherFormViewModel model)
        {
            teacher.FirstName = model.FirstName;
            teacher.LastName = model.LastName;
            teacher.Email = model.Email;
            teacher.Department = model.Department;
        }
    }
}
