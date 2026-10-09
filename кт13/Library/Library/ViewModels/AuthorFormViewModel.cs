using System.ComponentModel.DataAnnotations;

namespace Library.ViewModels
{
    public class AuthorFormViewModel
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

        [Range(1, 2100, ErrorMessage = "Некорректный год")]
        [Display(Name = "Год рождения")]
        public int? BirthYear { get; set; }

        [MaxLength(60)]
        [Display(Name = "Страна")]
        public string? Country { get; set; }

        [MaxLength(1000)]
        [Display(Name = "Биография")]
        public string? Biography { get; set; }
    }
}
