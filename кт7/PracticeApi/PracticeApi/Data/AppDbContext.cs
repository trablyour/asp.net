using Microsoft.EntityFrameworkCore;
using PracticeApi.Models;

namespace PracticeApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students => Set<Student>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<Resource> Resources => Set<Resource>();
        public DbSet<HotelRoom> HotelRooms => Set<HotelRoom>();
        public DbSet<RestaurantTable> RestaurantTables => Set<RestaurantTable>();
        public DbSet<Booking> Bookings => Set<Booking>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // статус храним в базе строкой
            modelBuilder.Entity<TaskItem>()
                .Property(t => t.Status)
                .HasConversion<string>();

            // все ресурсы в одной таблице, тип различается по колонке ResourceType
            modelBuilder.Entity<Resource>()
                .HasDiscriminator<string>("ResourceType")
                .HasValue<HotelRoom>("HotelRoom")
                .HasValue<RestaurantTable>("RestaurantTable");

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Resource)
                .WithMany()
                .HasForeignKey(b => b.ResourceId)
                .OnDelete(DeleteBehavior.Cascade);

            Seed(modelBuilder);
        }

        private static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, FirstName = "Иван", LastName = "Петров", Group = "ИС-21", Course = 3, Email = "petrov@mail.ru" },
                new Student { Id = 2, FirstName = "Анна", LastName = "Смирнова", Group = "ИС-21", Course = 3, Email = "smirnova@mail.ru" },
                new Student { Id = 3, FirstName = "Олег", LastName = "Кузнецов", Group = "ИС-22", Course = 2, Email = "kuznetsov@mail.ru" }
            );

            modelBuilder.Entity<TaskItem>().HasData(
                new TaskItem { Id = 1, Title = "Сделать макет главной", Description = "Набросать структуру страницы", Status = TaskItemStatus.Done, CreatedAt = new DateTime(2026, 10, 1, 10, 0, 0) },
                new TaskItem { Id = 2, Title = "Настроить базу данных", Description = "Создать таблицы и связи", Status = TaskItemStatus.InProgress, CreatedAt = new DateTime(2026, 10, 2, 12, 0, 0) },
                new TaskItem { Id = 3, Title = "Написать тесты", Description = null, Status = TaskItemStatus.New, CreatedAt = new DateTime(2026, 10, 3, 9, 30, 0) }
            );

            modelBuilder.Entity<HotelRoom>().HasData(
                new HotelRoom { Id = 1, Name = "Номер 101", Description = "Стандарт", Beds = 1, PricePerNight = 3500m },
                new HotelRoom { Id = 2, Name = "Номер 102", Description = "Стандарт", Beds = 2, PricePerNight = 4500m },
                new HotelRoom { Id = 3, Name = "Номер 201", Description = "Люкс с видом на город", Beds = 2, PricePerNight = 9000m }
            );

            modelBuilder.Entity<RestaurantTable>().HasData(
                new RestaurantTable { Id = 4, Name = "Столик 1", Seats = 2, NearWindow = true },
                new RestaurantTable { Id = 5, Name = "Столик 2", Seats = 4, NearWindow = false },
                new RestaurantTable { Id = 6, Name = "Столик 3", Seats = 6, NearWindow = true }
            );

            modelBuilder.Entity<Booking>().HasData(
                new Booking { Id = 1, ResourceId = 1, CustomerName = "Сидоров", StartTime = new DateTime(2026, 11, 10, 14, 0, 0), EndTime = new DateTime(2026, 11, 12, 12, 0, 0) },
                new Booking { Id = 2, ResourceId = 5, CustomerName = "Иванова", StartTime = new DateTime(2026, 11, 10, 19, 0, 0), EndTime = new DateTime(2026, 11, 10, 21, 0, 0) }
            );
        }
    }
}
