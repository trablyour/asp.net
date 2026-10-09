var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// Задание 2: маршрут для передачи имени через URL
app.MapControllerRoute(
    name: "greet",
    pattern: "Page/Greet/{name}",
    defaults: new { controller = "Page", action = "Greet" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
