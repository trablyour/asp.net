using BlazorDemo.Services;

var builder = WebApplication.CreateBuilder(args);

// классический Blazor Server: страница _Host + SignalR-хаб
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddSingleton<DataService>();

// scoped в Blazor Server = один экземпляр на подключение пользователя
builder.Services.AddScoped<LocalizationService>();
builder.Services.AddScoped<NavigationTracker>();
builder.Services.AddScoped<RouteTitleService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
