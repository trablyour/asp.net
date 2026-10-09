using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AdminPanel.Data;
using AdminPanel.Models;
using AdminPanel.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<RussianIdentityErrorDescriber>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// политики авторизации
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.AdminOnly, policy => policy.RequireRole(Roles.Admin));
    options.AddPolicy(Policies.RegisteredUser, policy => policy.RequireRole(Roles.Admin, Roles.User));
});

builder.Services.AddSingleton<SiteSettings>();

var app = builder.Build();

// база, роли и тестовые пользователи
await DbInitializer.SeedAsync(app.Services);

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
