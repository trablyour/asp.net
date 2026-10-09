# University — система управления учебным процессом

Веб-приложение на **ASP.NET Core MVC** и **Entity Framework Core**: преподаватели, курсы и студенты со связями «один ко многим» и «многие ко многим», каскадным удалением и полным CRUD.

## Модель данных

```
Teacher 1 ──── N Course N ──── N Student
                       (через Enrollment)
```

| Связь | Тип | Как устроена |
|---|---|---|
| Teacher → Course | один ко многим | у курса внешний ключ `TeacherId` |
| Student ↔ Course | многие ко многим | промежуточная сущность `Enrollment` с составным ключом `(StudentId, CourseId)` и датой записи |

Явная сущность `Enrollment` выбрана, чтобы хранить дату записи на курс; составной ключ не даёт записать студента на один курс дважды.

## Возможности

- CRUD преподавателей, студентов и курсов;
- запись студентов на курсы с обеих сторон: со страницы курса и со страницы студента;
- назначение преподавателя курсу (при создании, при редактировании или быстрой формой на странице курса);
- выбор курсов галочками при создании студента;
- фильтр студентов по группе;
- страница подтверждения удаления показывает, что удалится каскадом;
- главная страница со сводкой.

## Технологии

.NET 7 / 8 · ASP.NET Core MVC · Entity Framework Core · SQLite

## Запуск

```bash
dotnet run
```

Открыть http://localhost:5000. База `university.db` создаётся автоматически с тестовыми данными: 3 преподавателя, 4 курса, 5 студентов, 9 записей на курсы.

## Настройка базы данных

Используются **атрибуты** и **Fluent API**.

**Атрибуты** в моделях: `[Table]`, `[Key]`, `[Required]`, `[MaxLength]`, `[Range]`, `[NotMapped]`.

**Fluent API** в `Data/UniversityContext.cs`:

```csharp
// один ко многим
entity.HasMany(t => t.Courses)
      .WithOne(c => c.Teacher)
      .HasForeignKey(c => c.TeacherId)
      .OnDelete(DeleteBehavior.Cascade);

// многие ко многим
modelBuilder.Entity<Enrollment>(entity =>
{
    entity.HasKey(e => new { e.StudentId, e.CourseId });

    entity.HasOne(e => e.Student).WithMany(s => s.Enrollments)
          .HasForeignKey(e => e.StudentId).OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.Course).WithMany(c => c.Enrollments)
          .HasForeignKey(e => e.CourseId).OnDelete(DeleteBehavior.Cascade);
});
```

Также настроены уникальные индексы на email, значения по умолчанию (`Credits = 3`, `EnrolledAt = CURRENT_TIMESTAMP`) и начальные данные через `HasData`.

## Каскадное удаление

| Что удаляем | Что удаляется вместе с ним | Что остаётся |
|---|---|---|
| Преподавателя | его курсы и все записи студентов на эти курсы | студенты |
| Студента | его записи на курсы | курсы |
| Курс | записи студентов на этот курс | студенты, преподаватель |

## Структура

```
University/
├── Program.cs
├── Models/            Teacher, Student, Course, Enrollment
├── ViewModels/        формы создания и редактирования
├── Data/              UniversityContext (Fluent API, начальные данные)
├── Controllers/       Home, Teachers, Students, Courses
└── Views/             интерфейс
```

## Проверка

1. Открыть курс → добавить на него студента → убрать его с курса.
2. Открыть студента → записать на курс → отписать.
3. Сменить преподавателя курса.
4. Удалить преподавателя Волкова: страница подтверждения покажет 2 курса и 3 записи, которые удалятся. После удаления у студента Петрова пропадут эти курсы, а сам студент останется.

В консоли выводятся SQL-запросы EF Core, по ним видно, что происходит в базе.

<img width="990" height="655" alt="пупа1" src="https://github.com/user-attachments/assets/8e008cee-5dcb-4643-95d4-41544c336dad" />

<img width="1236" height="918" alt="пупа2" src="https://github.com/user-attachments/assets/54ebe01f-f918-4adc-bc76-177d043f15ae" />

<img width="1041" height="747" alt="пупа3" src="https://github.com/user-attachments/assets/c07ac807-3ace-4b96-b591-49abe6ad4033" />

<img width="1225" height="1076" alt="пупа4" src="https://github.com/user-attachments/assets/14ba003f-ae75-4ffb-9522-35ea4334e4e0" />

<img width="1282" height="1117" alt="пупа5" src="https://github.com/user-attachments/assets/b4e38419-529d-4ffa-9e6e-41a66157d5e2" />


