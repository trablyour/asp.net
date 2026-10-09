using System.Text.Json;
using Microsoft.JSInterop;
using TodoApp.Models;

namespace TodoApp.Services
{
    // сохраняет задачи в localStorage через JS interop
    public class TodoStorage
    {
        private const string StorageKey = "todo-items";
        private readonly IJSRuntime _js;

        public TodoStorage(IJSRuntime js)
        {
            _js = js;
        }

        public async Task<List<TodoItem>> LoadAsync()
        {
            string? json = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey);

            // первый запуск: показываем несколько примеров
            if (json == null)
                return CreateSamples();

            try
            {
                return JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new List<TodoItem>();
            }
            catch (JsonException)
            {
                // в хранилище мусор - начинаем с пустого списка
                return new List<TodoItem>();
            }
        }

        public async Task SaveAsync(List<TodoItem> items)
        {
            string json = JsonSerializer.Serialize(items);
            await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
        }

        private static List<TodoItem> CreateSamples()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            return new List<TodoItem>
            {
                new TodoItem { Title = "Сдать контрольную точку", Priority = Priority.High, DueDate = today.AddDays(2), CreatedAt = DateTime.Now.AddMinutes(-3) },
                new TodoItem { Title = "Сделать скриншоты для отчета", Priority = Priority.Medium, DueDate = today.AddDays(1), CreatedAt = DateTime.Now.AddMinutes(-2) },
                new TodoItem { Title = "Установить .NET SDK", Priority = Priority.Low, IsDone = true, CompletedAt = DateTime.Now.AddMinutes(-1), CreatedAt = DateTime.Now.AddMinutes(-1) }
            };
        }
    }
}
