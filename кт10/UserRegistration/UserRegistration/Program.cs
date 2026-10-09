using Microsoft.EntityFrameworkCore;
using UserRegistration.Data;
using UserRegistration.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// in-memory база: данные живут, пока запущено приложение
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("UsersDb"));

builder.Services.AddScoped<UserService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// пара пользователей для проверки API
using (var scope = app.Services.CreateScope())
{
    var users = scope.ServiceProvider.GetRequiredService<UserService>();
    await users.SeedAsync();
}

// любая необработанная ошибка превращается в ответ 500 с понятным текстом
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new { error = "Внутренняя ошибка сервера" });
    });
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
