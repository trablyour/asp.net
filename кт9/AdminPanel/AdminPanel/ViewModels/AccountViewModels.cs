using System.ComponentModel.DataAnnotations;

namespace AdminPanel.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = "";

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите имя")]
        [MaxLength(100)]
        [Display(Name = "Имя")]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        public string Email { get; set; } = "";

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
}
