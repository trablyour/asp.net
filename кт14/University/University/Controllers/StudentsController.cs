using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using University.Data;
using University.Models;
using University.ViewModels;

namespace University.Controllers
{
    public class StudentsController : Controller
    {
        private readonly UniversityContext _db;

        public StudentsController(UniversityContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(string? group)
        {
            var query = _db.Students.Include(s => s.Enrollments).AsQueryable();

            if (!string.IsNullOrWhiteSpace(group))
                query = query.Where(s => s.Group == group);

            ViewBag.Groups = await _db.Students.Select(s => s.Group).Distinct().OrderBy(g => g).ToListAsync();
            ViewBag.Group = group;

            return View(await query.OrderBy(s => s.LastName).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var student = await _db.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                        .ThenInclude(c => c.Teacher)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
                return NotFound();

            // курсы, на которые студент еще не записан
            var enrolledIds = student.Enrollments.Select(e => e.CourseId).ToList();
            var available = await _db.Courses
                .Where(c => !enrolledIds.Contains(c.Id))
                .OrderBy(c => c.Title)
                .ToListAsync();

            ViewBag.AvailableCourses = new SelectList(available, "Id", "Title");
            return View(student);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Courses = await _db.Courses.Include(c => c.Teacher).OrderBy(c => c.Title).ToListAsync();
            return View(new StudentFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentFormViewModel model)
        {
            await CheckEmailAsync(model);
            if (!ModelState.IsValid)
            {
                ViewBag.Courses = await _db.Courses.Include(c => c.Teacher).OrderBy(c => c.Title).ToListAsync();
                return View(model);
            }

            var student = new Student();
            Fill(student, model);

            // сразу записываем на выбранные курсы
            var validIds = await _db.Courses.Where(c => model.CourseIds.Contains(c.Id)).Select(c => c.Id).ToListAsync();
            foreach (var courseId in validIds)
                student.Enrollments.Add(new Enrollment { CourseId = courseId, EnrolledAt = DateTime.Now });

            _db.Students.Add(student);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Студент {student.FullName} добавлен";
            return RedirectToAction(nameof(Details), new { id = student.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _db.Students.FindAsync(id);
            if (student == null)
                return NotFound();

            return View(new StudentFormViewModel
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                Group = student.Group
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StudentFormViewModel model)
        {
            var student = await _db.Students.FindAsync(id);
            if (student == null)
                return NotFound();

            model.Id = id;
            await CheckEmailAsync(model);
            if (!ModelState.IsValid)
                return View(model);

            Fill(student, model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Данные студента сохранены";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _db.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
                return NotFound();

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _db.Students.FindAsync(id);
            if (student == null)
                return NotFound();

            // записи на курсы удалит каскад, сами курсы останутся
            _db.Students.Remove(student);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Студент {student.FullName} удален";
            return RedirectToAction(nameof(Index));
        }

        // записать студента на курс
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int id, int courseId)
        {
            if (!await _db.Students.AnyAsync(s => s.Id == id) || !await _db.Courses.AnyAsync(c => c.Id == courseId))
                return NotFound();

            if (await _db.Enrollments.AnyAsync(e => e.StudentId == id && e.CourseId == courseId))
            {
                TempData["Error"] = "Студент уже записан на этот курс";
            }
            else
            {
                _db.Enrollments.Add(new Enrollment { StudentId = id, CourseId = courseId, EnrolledAt = DateTime.Now });
                await _db.SaveChangesAsync();
                TempData["Success"] = "Студент записан на курс";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // отписать студента от курса
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unenroll(int id, int courseId)
        {
            var enrollment = await _db.Enrollments.FindAsync(id, courseId);
            if (enrollment == null)
                return NotFound();

            _db.Enrollments.Remove(enrollment);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Студент отписан от курса";
            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task CheckEmailAsync(StudentFormViewModel model)
        {
            if (await _db.Students.AnyAsync(s => s.Email == model.Email && s.Id != model.Id))
                ModelState.AddModelError(nameof(model.Email), "Этот email уже используется");
        }

        private static void Fill(Student student, StudentFormViewModel model)
        {
            student.FirstName = model.FirstName;
            student.LastName = model.LastName;
            student.Email = model.Email;
            student.Group = model.Group;
        }
    }
}
