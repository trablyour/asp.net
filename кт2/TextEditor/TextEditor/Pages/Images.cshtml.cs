using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TextEditor.Services;

namespace TextEditor.Pages
{
    public class ImagesModel : PageModel
    {
        private readonly TextStore _store;
        private readonly IWebHostEnvironment _env;
        private const string FileName = "images.html";

        private static readonly string[] AllowedExt = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public ImagesModel(TextStore store, IWebHostEnvironment env)
        {
            _store = store;
            _env = env;
        }

        [BindProperty]
        public string? Text { get; set; }

        public string? Message { get; set; }

        public void OnGet()
        {
            Text = _store.Load(FileName);
        }

        public void OnPost()
        {
            _store.Save(FileName, Text);
            Message = "Текст сохранен";
        }

        // сюда TinyMCE отправляет картинки
        public async Task<IActionResult> OnPostUploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest();

            string ext = Path.GetExtension(file.FileName).ToLower();
            if (!AllowedExt.Contains(ext))
                return BadRequest();

            string folder = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(folder);

            string newName = Guid.NewGuid().ToString("N") + ext;
            string path = Path.Combine(folder, newName);

            using (var stream = System.IO.File.Create(path))
            {
                await file.CopyToAsync(stream);
            }

            return new JsonResult(new { location = "/uploads/" + newName });
        }
    }
}
