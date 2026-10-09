using System.ComponentModel.DataAnnotations;

namespace University.ViewModels
{
    public class TeacherFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите имя")]
        [MaxLength(50)]
        [Display(Name = "Имя")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Введите фамилию")]
        [MaxLength(50)]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        [MaxLength(100)]
        public string Email { get; set; } = "";

        [MaxLength(100)]
        [Display(Name = "Кафедра")]
        public string? Department { get; set; }
    }

    public class StudentFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите имя")]
        [MaxLength(50)]
        [Display(Name = "Имя")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Введите фамилию")]
        [MaxLength(50)]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        [MaxLength(100)]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Введите группу")]
        [MaxLength(20)]
        [Display(Name = "Группа")]
        public string Group { get; set; } = "";

        // курсы, отмеченные галочками на форме
        [Display(Name = "Курсы")]
        public List<int> CourseIds { get; set; } = new();
    }

    public class CourseFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название")]
        [MaxLength(150)]
        [Display(Name = "Название")]
        public string Title { get; set; } = "";

        [MaxLength(1000)]
        [Display(Name = "Описание")]
        public string? Description { get; set; }

        [Range(1, 20, ErrorMessage = "От 1 до 20")]
        [Display(Name = "Кредиты")]
        public int Credits { get; set; } = 3;

        [Range(1, int.MaxValue, ErrorMessage = "Выберите преподавателя")]
        [Display(Name = "Преподаватель")]
        public int TeacherId { get; set; }
    }
}
