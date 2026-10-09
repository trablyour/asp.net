using Microsoft.AspNetCore.Components;
using BlazorDemo.Services;

namespace BlazorDemo.Components
{
    // базовый класс: компонент перерисовывается при смене языка
    public abstract class LocalizedComponentBase : ComponentBase, IDisposable
    {
        [Inject]
        protected LocalizationService L { get; set; } = null!;

        protected override void OnInitialized()
        {
            L.Changed += OnLanguageChanged;
        }

        private void OnLanguageChanged()
        {
            InvokeAsync(StateHasChanged);
        }

        public virtual void Dispose()
        {
            L.Changed -= OnLanguageChanged;
        }
    }
}
