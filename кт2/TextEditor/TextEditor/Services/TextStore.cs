namespace TextEditor.Services
{
    // хранит тексты в файлах в папке Data
    public class TextStore
    {
        private readonly string _folder;

        public TextStore(IWebHostEnvironment env)
        {
            _folder = Path.Combine(env.ContentRootPath, "Data");
            Directory.CreateDirectory(_folder);
        }

        public string Load(string fileName)
        {
            string path = Path.Combine(_folder, fileName);
            if (!File.Exists(path))
                return "";
            return File.ReadAllText(path);
        }

        public void Save(string fileName, string? text)
        {
            string path = Path.Combine(_folder, fileName);
            File.WriteAllText(path, text ?? "");
        }
    }
}
