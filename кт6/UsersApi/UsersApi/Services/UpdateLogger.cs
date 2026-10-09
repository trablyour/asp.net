namespace UsersApi.Services
{
    // задание 5: пишет каждую попытку обновления в файл Logs/updates.log
    public class UpdateLogger
    {
        private readonly string _filePath;
        private readonly object _lock = new object();

        public UpdateLogger(IWebHostEnvironment env)
        {
            string folder = Path.Combine(env.ContentRootPath, "Logs");
            Directory.CreateDirectory(folder);
            _filePath = Path.Combine(folder, "updates.log");
        }

        public void Write(string action, int userId, bool success, string details)
        {
            string status = success ? "УСПЕШНО" : "ОШИБКА";
            string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {action} | UserId={userId} | {status} | {details}";

            lock (_lock)
            {
                File.AppendAllText(_filePath, line + Environment.NewLine);
            }
        }

        public string ReadAll()
        {
            lock (_lock)
            {
                if (!File.Exists(_filePath))
                    return "";
                return File.ReadAllText(_filePath);
            }
        }
    }
}
