using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace BlazorDemo.Services
{
    // глобальный обработчик навигации: логирует каждый переход и сообщает о нем подписчикам
    public class NavigationTracker : IDisposable
    {
        private readonly NavigationManager _nav;
        private readonly ILogger<NavigationTracker> _logger;

        public string CurrentPath { get; private set; }
        public List<string> History { get; } = new();

        public event Action<string>? RouteChanged;

        public NavigationTracker(NavigationManager nav, ILogger<NavigationTracker> logger)
        {
            _nav = nav;
            _logger = logger;

            CurrentPath = ToPath(_nav.Uri);
            History.Add(CurrentPath);

            _nav.LocationChanged += OnLocationChanged;
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            CurrentPath = ToPath(e.Location);

            History.Insert(0, CurrentPath);
            if (History.Count > 10)
                History.RemoveAt(History.Count - 1);

            _logger.LogInformation("Навигация: {Path} (переход по ссылке: {Intercepted})",
                CurrentPath, e.IsNavigationIntercepted);

            RouteChanged?.Invoke(CurrentPath);
        }

        private string ToPath(string uri) => "/" + _nav.ToBaseRelativePath(uri);

        public void Dispose()
        {
            _nav.LocationChanged -= OnLocationChanged;
        }
    }
}
