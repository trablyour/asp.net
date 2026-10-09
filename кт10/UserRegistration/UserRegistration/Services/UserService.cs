using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserRegistration.Data;
using UserRegistration.Models;

namespace UserRegistration.Services
{
    // работа с пользователями, общая для формы и API
    public class UserService
    {
        private readonly AppDbContext _db;
        private readonly PasswordHasher<User> _hasher = new PasswordHasher<User>();

        public UserService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _db.Users.OrderBy(u => u.Id).ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _db.Users.FindAsync(id);
        }

        // exceptId нужен при обновлении, чтобы не сравнивать пользователя с самим собой
        public async Task<bool> UsernameTakenAsync(string username, int? exceptId = null)
        {
            string name = username.ToLower();
            return await _db.Users.AnyAsync(u => u.Username.ToLower() == name && u.Id != exceptId);
        }

        public async Task<bool> EmailTakenAsync(string email, int? exceptId = null)
        {
            string value = email.ToLower();
            return await _db.Users.AnyAsync(u => u.Email.ToLower() == value && u.Id != exceptId);
        }

        public async Task<User> CreateAsync(string username, string email, string password)
        {
            var user = new User
            {
                Username = username,
                Email = email,
                CreatedAt = DateTime.Now
            };
            user.PasswordHash = _hasher.HashPassword(user, password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task UpdateAsync(User user, string username, string email, string? password)
        {
            user.Username = username;
            user.Email = email;

            if (!string.IsNullOrEmpty(password))
                user.PasswordHash = _hasher.HashPassword(user, password);

            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }

        public async Task SeedAsync()
        {
            if (await _db.Users.AnyAsync())
                return;

            await CreateAsync("Ivan", "ivan@mail.ru", "password1");
            await CreateAsync("Anna", "anna@mail.ru", "password2");
        }
    }
}
