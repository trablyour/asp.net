using Microsoft.EntityFrameworkCore;
using University.Models;

namespace University.Data
{
    public class UniversityContext : DbContext
    {
        public UniversityContext(DbContextOptions<UniversityContext> options) : base(options)
        {
        }

        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ---------- Teacher ----------
            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasIndex(t => t.Email).IsUnique();

                // один ко многим: преподаватель -> курсы
                // удаляем преподавателя - удаляются его курсы (а с ними и записи студентов)
                entity.HasMany(t => t.Courses)
                      .WithOne(c => c.Teacher)
                      .HasForeignKey(c => c.TeacherId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ---------- Student ----------
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasIndex(s => s.Email).IsUnique();
                entity.HasIndex(s => s.Group);
            });

            // ---------- Course ----------
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasIndex(c => c.Title);

                entity.Property(c => c.Credits)
                      .HasDefaultValue(3);
            });

            // ---------- Enrollment (многие ко многим) ----------
            modelBuilder.Entity<Enrollment>(entity =>
            {
                // составной ключ: студент не может записаться на один курс дважды
                entity.HasKey(e => new { e.StudentId, e.CourseId });

                entity.HasOne(e => e.Student)
                      .WithMany(s => s.Enrollments)
                      .HasForeignKey(e => e.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Course)
                      .WithMany(c => c.Enrollments)
                      .HasForeignKey(e => e.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.EnrolledAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            Seed(modelBuilder);
        }

        private static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Teacher>().HasData(
                new Teacher { Id = 1, FirstName = "Сергей", LastName = "Волков", Email = "volkov@college.ru", Department = "Программирование" },
                new Teacher { Id = 2, FirstName = "Елена", LastName = "Орлова", Email = "orlova@college.ru", Department = "Базы данных" },
                new Teacher { Id = 3, FirstName = "Андрей", LastName = "Соколов", Email = "sokolov@college.ru", Department = "Математика" }
            );

            modelBuilder.Entity<Course>().HasData(
                new Course { Id = 1, Title = "Основы C#", Description = "Синтаксис, ООП, коллекции", Credits = 5, TeacherId = 1 },
                new Course { Id = 2, Title = "ASP.NET Core", Description = "MVC, Web API, Entity Framework", Credits = 6, TeacherId = 1 },
                new Course { Id = 3, Title = "Проектирование БД", Description = "Нормализация, SQL, индексы", Credits = 4, TeacherId = 2 },
                new Course { Id = 4, Title = "Дискретная математика", Description = "Множества, графы, логика", Credits = 3, TeacherId = 3 }
            );

            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, FirstName = "Иван", LastName = "Петров", Email = "petrov@mail.ru", Group = "ИС-21" },
                new Student { Id = 2, FirstName = "Анна", LastName = "Смирнова", Email = "smirnova@mail.ru", Group = "ИС-21" },
                new Student { Id = 3, FirstName = "Олег", LastName = "Кузнецов", Email = "kuznetsov@mail.ru", Group = "ИС-22" },
                new Student { Id = 4, FirstName = "Мария", LastName = "Попова", Email = "popova@mail.ru", Group = "ИС-22" },
                new Student { Id = 5, FirstName = "Дмитрий", LastName = "Лебедев", Email = "lebedev@mail.ru", Group = "ИС-23" }
            );

            var date = new DateTime(2026, 9, 1, 9, 0, 0);
            modelBuilder.Entity<Enrollment>().HasData(
                new Enrollment { StudentId = 1, CourseId = 1, EnrolledAt = date },
                new Enrollment { StudentId = 1, CourseId = 2, EnrolledAt = date },
                new Enrollment { StudentId = 1, CourseId = 3, EnrolledAt = date },
                new Enrollment { StudentId = 2, CourseId = 1, EnrolledAt = date },
                new Enrollment { StudentId = 2, CourseId = 4, EnrolledAt = date },
                new Enrollment { StudentId = 3, CourseId = 2, EnrolledAt = date },
                new Enrollment { StudentId = 3, CourseId = 3, EnrolledAt = date },
                new Enrollment { StudentId = 4, CourseId = 3, EnrolledAt = date },
                new Enrollment { StudentId = 4, CourseId = 4, EnrolledAt = date }
            );
        }
    }
}
