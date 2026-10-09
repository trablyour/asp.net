using Microsoft.EntityFrameworkCore;
using ShopApi.Models;

namespace ShopApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // начальные данные, попадут в миграцию
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Электроника", Description = "Телефоны, ноутбуки, аксессуары" },
                new Category { Id = 2, Name = "Книги", Description = "Художественная и учебная литература" },
                new Category { Id = 3, Name = "Одежда", Description = "Мужская и женская одежда" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Смартфон", Description = "6.1 дюйма, 128 ГБ", Price = 29990m, Stock = 15, CategoryId = 1 },
                new Product { Id = 2, Name = "Ноутбук", Description = "15.6 дюйма, 16 ГБ ОЗУ", Price = 64990m, Stock = 7, CategoryId = 1 },
                new Product { Id = 3, Name = "Наушники", Description = "Беспроводные", Price = 4990m, Stock = 30, CategoryId = 1 },
                new Product { Id = 4, Name = "Чистый код", Description = "Роберт Мартин", Price = 1200m, Stock = 20, CategoryId = 2 },
                new Product { Id = 5, Name = "Мастер и Маргарита", Description = "Михаил Булгаков", Price = 650m, Stock = 12, CategoryId = 2 },
                new Product { Id = 6, Name = "Футболка", Description = "Хлопок, размер M", Price = 990m, Stock = 40, CategoryId = 3 }
            );
        }
    }
}
