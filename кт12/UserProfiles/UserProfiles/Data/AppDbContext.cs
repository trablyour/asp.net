using Microsoft.EntityFrameworkCore;
using UserProfiles.Models;

namespace UserProfiles.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ---------- User ----------
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();

                entity.Property(u => u.CreatedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // связь один к одному
                entity.HasOne(u => u.Profile)
                      .WithOne(p => p.User)
                      .HasForeignKey<UserProfile>(p => p.UserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade); // удалили пользователя - удалился профиль
            });

            // ---------- UserProfile ----------
            modelBuilder.Entity<UserProfile>(entity =>
            {
                // уникальный индекс на UserId гарантирует, что у пользователя не будет двух профилей
                entity.HasIndex(p => p.UserId).IsUnique();

                entity.Property(p => p.City)
                      .HasDefaultValue("Не указан");
            });

            Seed(modelBuilder);
        }

        private static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "ivanov", Email = "ivanov@mail.ru", CreatedAt = new DateTime(2026, 9, 1, 10, 0, 0) },
                new User { Id = 2, Username = "smirnova", Email = "smirnova@mail.ru", CreatedAt = new DateTime(2026, 9, 5, 14, 30, 0) },
                new User { Id = 3, Username = "kuznetsov", Email = "kuznetsov@mail.ru", CreatedAt = new DateTime(2026, 9, 20, 9, 15, 0) }
            );

            // у третьего пользователя профиля нет, чтобы показать его создание
            modelBuilder.Entity<UserProfile>().HasData(
                new UserProfile { Id = 1, UserId = 1, FirstName = "Иван", LastName = "Иванов", BirthDate = new DateTime(2003, 4, 12), Phone = "+7 900 111-22-33", City = "Москва", Bio = "Студент, увлекаюсь программированием" },
                new UserProfile { Id = 2, UserId = 2, FirstName = "Анна", LastName = "Смирнова", BirthDate = new DateTime(2002, 11, 3), Phone = "+7 900 444-55-66", City = "Казань", Bio = "Люблю дизайн и фотографию" }
            );
        }
    }
}
