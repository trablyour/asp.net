using Microsoft.AspNetCore.Identity;
using AdminPanel.Models;

namespace AdminPanel.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.EnsureCreatedAsync();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // роли
            foreach (var role in Roles.All)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // тестовые аккаунты
            await CreateUserAsync(userManager, "admin@site.ru", "Администратор", "Admin123", Roles.Admin);
            await CreateUserAsync(userManager, "user@site.ru", "Иван Петров", "User123", Roles.User);
        }

        private static async Task CreateUserAsync(UserManager<ApplicationUser> userManager,
            string email, string fullName, string password, string role)
        {
            if (await userManager.FindByEmailAsync(email) != null)
                return;

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, role);
        }
    }
}
