namespace AdminPanel.Services
{
    // настройки сайта, меняются в админке (хранятся в памяти)
    public class SiteSettings
    {
        public string SiteName { get; set; } = "Мой сайт";
        public bool AllowRegistration { get; set; } = true;
    }
}
