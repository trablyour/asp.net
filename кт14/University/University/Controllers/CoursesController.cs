using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using University.Data;
using University.Models;
using University.ViewModels;

namespace University.Controllers
{
    public class CoursesController : Controller
    {
        private readonly UniversityContext _db;

        public CoursesController(UniversityContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _db.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Enrollments)
                .OrderBy(c => c.Title)
                .ToListAsync();

            return View(courses);
        }

        public async Task<IActionResult> Details(int id)
        {
            var course = await _db.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound();

            // студенты, которых еще нет на курсе
            var enrolledIds = course.Enrollments.Select(e => e.StudentId).ToList();
            var available = await _db.Students
                .Where(s => !enrolledIds.Contains(s.Id))
                .OrderBy(s => s.LastName)
                .Select(s => new { s.Id, Name = s.LastName + " " + s.FirstName + " (" + s.Group + ")" })
                .ToListAsync();

            ViewBag.AvailableStudents = new SelectList(available, "Id", "Name");
            ViewBag.Teachers = await TeachersSelectList(course.TeacherId);
            return View(course);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? teacherId)
        {
            if (!await _db.Teachers.AnyAsync())
            {
                TempData["Error"] = "Сначала добавьте преподавателя";
                return RedirectToAction("Create", "Teachers");
            }

            var model = new CourseFormViewModel { TeacherId = teacherId ?? 0 };
            ViewBag.Teachers = await TeachersSelectList(model.TeacherId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseFormViewModel model)
        {
            await CheckTeacherAsync(model);
            if (!ModelState.IsValid)
            {
                ViewBag.Teachers = await TeachersSelectList(model.TeacherId);
                return View(model);
            }

            var course = new Course();
            Fill(course, model);

            _db.Courses.Add(course);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Курс «{course.Title}» создан";
            return RedirectToAction(nameof(Details), new { id = course.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _db.Courses.FindAsync(id);
            if (course == null)
                return NotFound();

            var model = new CourseFormViewModel
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Credits = course.Credits,
                TeacherId = course.TeacherId
            };

            ViewBag.Teachers = await TeachersSelectList(model.TeacherId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseFormViewModel model)
        {
            var course = await _db.Courses.FindAsync(id);
            if (course == null)
                return NotFound();

            model.Id = id;
            await CheckTeacherAsync(model);
            if (!ModelState.IsValid)
            {
                ViewBag.Teachers = await TeachersSelectList(model.TeacherId);
                return View(model);
            }

            Fill(course, model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Курс сохранен";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _db.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound();

            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _db.Courses.FindAsync(id);
            if (course == null)
                return NotFound();

            // записи студентов на курс удалит каскад
            _db.Courses.Remove(course);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Курс «{course.Title}» удален";
            return RedirectToAction(nameof(Index));
        }

        // добавить студента на курс
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStudent(int id, int studentId)
        {
            if (!await _db.Courses.AnyAsync(c => c.Id == id) || !await _db.Students.AnyAsync(s => s.Id == studentId))
                return NotFound();

            if (await _db.Enrollments.AnyAsync(e => e.CourseId == id && e.StudentId == studentId))
            {
                TempData["Error"] = "Студент уже записан на этот курс";
            }
            else
            {
                _db.Enrollments.Add(new Enrollment { CourseId = id, StudentId = studentId, EnrolledAt = DateTime.Now });
                await _db.SaveChangesAsync();
                TempData["Success"] = "Студент добавлен на курс";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // убрать студента с курса
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveStudent(int id, int studentId)
        {
            var enrollment = await _db.Enrollments.FindAsync(studentId, id);
            if (enrollment == null)
                return NotFound();

            _db.Enrollments.Remove(enrollment);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Студент убран с курса";
            return RedirectToAction(nameof(Details), new { id });
        }

        // назначить другого преподавателя
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTeacher(int id, int teacherId)
        {
            var course = await _db.Courses.FindAsync(id);
            if (course == null)
                return NotFound();

            var teacher = await _db.Teachers.FindAsync(teacherId);
            if (teacher == null)
            {
                TempData["Error"] = "Преподаватель не найден";
                return RedirectToAction(nameof(Details), new { id });
            }

            course.TeacherId = teacherId;
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Преподавателем курса назначен {teacher.FullName}";
            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task CheckTeacherAsync(CourseFormViewModel model)
        {
            if (model.TeacherId > 0 && !await _db.Teachers.AnyAsync(t => t.Id == model.TeacherId))
                ModelState.AddModelError(nameof(model.TeacherId), "Такого преподавателя нет");
        }

        private async Task<SelectList> TeachersSelectList(int? selectedId)
        {
            var teachers = await _db.Teachers
                .OrderBy(t => t.LastName)
                .Select(t => new { t.Id, Name = t.LastName + " " + t.FirstName })
                .ToListAsync();

            return new SelectList(teachers, "Id", "Name", selectedId);
        }

        private static void Fill(Course course, CourseFormViewModel model)
        {
            course.Title = model.Title;
            course.Description = model.Description;
            course.Credits = model.Credits;
            course.TeacherId = model.TeacherId;
        }
    }
}
