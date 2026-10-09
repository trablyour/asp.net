using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TextEditor.Services;

namespace TextEditor.Pages
{
    public class FormatModel : PageModel
    {
        private readonly TextStore _store;
        private const string FileName = "formatted.html";

        public FormatModel(TextStore store)
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
