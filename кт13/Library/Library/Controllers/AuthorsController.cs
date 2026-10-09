using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library.Data;
using Library.Models;
using Library.ViewModels;

namespace Library.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly LibraryContext _db;

        public AuthorsController(LibraryContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var authors = await _db.Authors
                .Include(a => a.Books)
                .OrderBy(a => a.LastName)
                .ToListAsync();

            return View(authors);
        }

        public async Task<IActionResult> Details(int id)
        {
            var author = await _db.Authors
                .Include(a => a.Books.OrderBy(b => b.Year))
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
                return NotFound();

            return View(author);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new AuthorFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AuthorFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var author = new Author();
            Fill(author, model);

            _db.Authors.Add(author);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Автор {author.FullName} добавлен";
            return RedirectToAction(nameof(Details), new { id = author.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var author = await _db.Authors.FindAsync(id);
            if (author == null)
                return NotFound();

            var model = new AuthorFormViewModel
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName,
                BirthYear = author.BirthYear,
                Country = author.Country,
                Biography = author.Biography
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AuthorFormViewModel model)
        {
            var author = await _db.Authors.FindAsync(id);
            if (author == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                model.Id = id;
                return View(model);
            }

            Fill(author, model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Данные автора сохранены";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _db.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
                return NotFound();

            return View(author);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = await _db.Authors.FindAsync(id);
            if (author == null)
                return NotFound();

            int booksCount = await _db.Books.CountAsync(b => b.AuthorId == id);

            // книги отдельно не удаляем, это делает каскад
            _db.Authors.Remove(author);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Автор {author.FullName} удален, вместе с ним удалено книг: {booksCount}";
            return RedirectToAction(nameof(Index));
        }

        private static void Fill(Author author, AuthorFormViewModel model)
        {
            author.FirstName = model.FirstName;
            author.LastName = model.LastName;
            author.BirthYear = model.BirthYear;
            author.Country = model.Country;
            author.Biography = model.Biography;
        }
    }
}
