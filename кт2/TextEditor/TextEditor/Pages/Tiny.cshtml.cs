using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TextEditor.Services;

namespace TextEditor.Pages
{
    public class TinyModel : PageModel
    {
        private readonly TextStore _store;
        private const string FileName = "tiny.html";

        public TinyModel(TextStore store)
        {
            _store = store;
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
    }
}
