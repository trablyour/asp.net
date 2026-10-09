using System.ComponentModel.DataAnnotations;

namespace AdminPanel.ViewModels
{
    public class UserListItemViewModel
    {
        public string Id { get; set; } = "";
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Roles { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }

    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Введите имя")]
        [MaxLength(100)]
        [Display(Name = "Имя")]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Выберите роль")]
        [Display(Name = "Роль")]
        public string Role { get; set; } = "User";

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Повторите пароль")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Повторите пароль")]
        public string ConfirmPassword { get; set; } = "";
    }

    public class EditUserViewModel
    {
        [Required]
        public string Id { get; set; } = "";

        [Required(ErrorMessage = "Введите имя")]
        [MaxLength(100)]
        [Display(Name = "Имя")]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Выберите роль")]
        [Display(Name = "Роль")]
        public string Role { get; set; } = "User";

        // если пусто, пароль не меняется
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string? NewPassword { get; set; }
    }

    public class SettingsViewModel
    {
        [Required(ErrorMessage = "Введите название сайта")]
        [MaxLength(50)]
        [Display(Name = "Название сайта")]
        public string SiteName { get; set; } = "";

        [Display(Name = "Разрешить регистрацию новых пользователей")]
        public bool AllowRegistration { get; set; }
    }

    public class ReportsViewModel
    {
        public int TotalUsers { get; set; }
        public int AdminCount { get; set; }
        public int UserCount { get; set; }
        public int NewLast7Days { get; set; }
        public List<UserListItemViewModel> LatestUsers { get; set; } = new();
    }
}
