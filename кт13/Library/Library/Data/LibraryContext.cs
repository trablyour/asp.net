using Microsoft.EntityFrameworkCore;
using Library.Models;

namespace Library.Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
        {
        }

        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Book> Books => Set<Book>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // подключаем все классы конфигурации из папки Configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryContext).Assembly);

            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, FirstName = "Александр", LastName = "Пушкин", BirthYear = 1799, Country = "Россия", Biography = "Поэт, драматург и прозаик, основоположник современного русского литературного языка." },
                new Author { Id = 2, FirstName = "Лев", LastName = "Толстой", BirthYear = 1828, Country = "Россия", Biography = "Писатель и мыслитель, автор романов-эпопей." },
                new Author { Id = 3, FirstName = "Михаил", LastName = "Булгаков", BirthYear = 1891, Country = "Россия", Biography = "Писатель, драматург, театральный режиссер." },
                new Author { Id = 4, FirstName = "Джордж", LastName = "Оруэлл", BirthYear = 1903, Country = "Великобритания", Biography = "Писатель и публицист, автор антиутопий." }
            );

            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Евгений Онегин", Year = 1833, Genre = "Роман в стихах", Pages = 320, AuthorId = 1 },
                new Book { Id = 2, Title = "Капитанская дочка", Year = 1836, Genre = "Повесть", Pages = 190, AuthorId = 1 },
                new Book { Id = 3, Title = "Пиковая дама", Year = 1834, Genre = "Повесть", Pages = 60, AuthorId = 1 },
                new Book { Id = 4, Title = "Война и мир", Year = 1869, Genre = "Роман-эпопея", Pages = 1300, AuthorId = 2 },
                new Book { Id = 5, Title = "Анна Каренина", Year = 1877, Genre = "Роман", Pages = 860, AuthorId = 2 },
                new Book { Id = 6, Title = "Мастер и Маргарита", Year = 1967, Genre = "Роман", Pages = 480, AuthorId = 3 },
                new Book { Id = 7, Title = "Собачье сердце", Year = 1925, Genre = "Повесть", Pages = 160, AuthorId = 3 },
                new Book { Id = 8, Title = "1984", Year = 1949, Genre = "Антиутопия", Pages = 320, AuthorId = 4 },
                new Book { Id = 9, Title = "Скотный двор", Year = 1945, Genre = "Сатира", Pages = 140, AuthorId = 4 }
            );
        }
    }
}
