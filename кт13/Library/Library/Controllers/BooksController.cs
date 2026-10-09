using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library.Data;
using Library.Models;
using Library.ViewModels;

namespace Library.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryContext _db;

        public BooksController(LibraryContext db)
        {
            _db = db;
        }

        // список книг с поиском по названию и фильтром по автору
        public async Task<IActionResult> Index(string? search, int? authorId)
        {
            var query = _db.Books.Include(b => b.Author).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(b => b.Title.Contains(search));

            if (authorId != null)
                query = query.Where(b => b.AuthorId == authorId);

            var books = await query
                .OrderBy(b => b.Author.LastName)
                .ThenBy(b => b.Year)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.Authors = await AuthorsSelectList(authorId);
            return View(books);
        }

        public async Task<IActionResult> Details(int id)
        {
            var book = await _db.Books
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            return View(book);
        }

        // authorId можно передать, чтобы автор был сразу выбран (кнопка на странице автора)
        [HttpGet]
        public async Task<IActionResult> Create(int? authorId)
        {
            if (!await _db.Authors.AnyAsync())
            {
                TempData["Error"] = "Сначала добавьте хотя бы одного автора";
                return RedirectToAction("Create", "Authors");
            }

            var model = new BookFormViewModel { AuthorId = authorId ?? 0 };
            ViewBag.Authors = await AuthorsSelectList(model.AuthorId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookFormViewModel model)
        {
            await ValidateAsync(model);

            if (!ModelState.IsValid)
            {
                ViewBag.Authors = await AuthorsSelectList(model.AuthorId);
                return View(model);
            }

            var book = new Book();
            Fill(book, model);

            _db.Books.Add(book);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Книга «{book.Title}» добавлена";
            return RedirectToAction(nameof(Details), new { id = book.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            var model = new BookFormViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Year = book.Year,
                Genre = book.Genre,
                Pages = book.Pages,
                Isbn = book.Isbn,
                AuthorId = book.AuthorId
            };

            ViewBag.Authors = await AuthorsSelectList(model.AuthorId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookFormViewModel model)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            model.Id = id;
            await ValidateAsync(model);

            if (!ModelState.IsValid)
            {
                ViewBag.Authors = await AuthorsSelectList(model.AuthorId);
                return View(model);
            }

            Fill(book, model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Книга сохранена";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _db.Books
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            _db.Books.Remove(book);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Книга «{book.Title}» удалена";
            return RedirectToAction(nameof(Index));
        }

        private async Task ValidateAsync(BookFormViewModel model)
        {
            if (model.AuthorId > 0 && !await _db.Authors.AnyAsync(a => a.Id == model.AuthorId))
                ModelState.AddModelError(nameof(model.AuthorId), "Такого автора нет");

            if (!string.IsNullOrWhiteSpace(model.Isbn) &&
                await _db.Books.AnyAsync(b => b.Isbn == model.Isbn && b.Id != model.Id))
                ModelState.AddModelError(nameof(model.Isbn), "Книга с таким ISBN уже есть");
        }

        private async Task<SelectList> AuthorsSelectList(int? selectedId)
        {
            var authors = await _db.Authors
                .OrderBy(a => a.LastName)
                .Select(a => new { a.Id, Name = a.LastName + " " + a.FirstName })
                .ToListAsync();

            return new SelectList(authors, "Id", "Name", selectedId);
        }

        private static void Fill(Book book, BookFormViewModel model)
        {
            book.Title = model.Title;
            book.Year = model.Year;
            book.Genre = string.IsNullOrWhiteSpace(model.Genre) ? "Не указан" : model.Genre;
            book.Pages = model.Pages;
            book.Isbn = string.IsNullOrWhiteSpace(model.Isbn) ? null : model.Isbn;
            book.AuthorId = model.AuthorId;
        }
    }
}
