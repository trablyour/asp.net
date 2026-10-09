using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Services;
using TodoApp;
using TodoApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// хранение задач в localStorage браузера
builder.Services.AddScoped<TodoStorage>();

// сервис для отложенной загрузки сборок
builder.Services.AddScoped<LazyAssemblyLoader>();

await builder.Build().RunAsync();
