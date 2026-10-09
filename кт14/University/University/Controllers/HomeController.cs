using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University.Data;

namespace University.Controllers
{
    public class HomeController : Controller
    {
        private readonly UniversityContext _db;

        public HomeController(UniversityContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Teachers = await _db.Teachers.CountAsync();
            ViewBag.Students = await _db.Students.CountAsync();
            ViewBag.Courses = await _db.Courses.CountAsync();
            ViewBag.Enrollments = await _db.Enrollments.CountAsync();
            return View();
        }
    }
}
