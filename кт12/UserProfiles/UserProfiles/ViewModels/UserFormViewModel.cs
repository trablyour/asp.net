using System.ComponentModel.DataAnnotations;

namespace UserProfiles.ViewModels
{
    // форма пользователя (при создании сразу можно заполнить профиль)
    public class UserFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите логин")]
        [MaxLength(50, ErrorMessage = "Не длиннее 50 символов")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Только латинские буквы, цифры и _")]
        [Display(Name = "Логин")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        [MaxLength(100)]
        public string Email { get; set; } = "";

        // профиль при создании необязателен
        [Display(Name = "Создать профиль")]
        public bool CreateProfile { get; set; } = true;

        public ProfileFormViewModel Profile { get; set; } = new();
    }

    public class ProfileFormViewModel
    {
        public int UserId { get; set; }
        public string? Username { get; set; }

        [Required(ErrorMessage = "Введите имя")]
        [MaxLength(50)]
        [Display(Name = "Имя")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Введите фамилию")]
        [MaxLength(50)]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; } = "";

        [DataType(DataType.Date)]
        [Display(Name = "Дата рождения")]
        public DateTime? BirthDate { get; set; }

        [MaxLength(20)]
        [Phone(ErrorMessage = "Некорректный телефон")]
        [Display(Name = "Телефон")]
        public string? Phone { get; set; }

        [MaxLength(100)]
        [Display(Name = "Город")]
        public string? City { get; set; }

        [MaxLength(500)]
        [Display(Name = "О себе")]
        public string? Bio { get; set; }
    }
}
