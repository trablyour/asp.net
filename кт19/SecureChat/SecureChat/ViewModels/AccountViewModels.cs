using System.ComponentModel.DataAnnotations;

namespace SecureChat.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите логин")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Логин от 3 до 20 символов")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Только латинские буквы, цифры и _")]
        [Display(Name = "Логин")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Введите пароль")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль не короче 6 символов")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).+$", ErrorMessage = "Пароль должен содержать буквы и цифры")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Повторите пароль")]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Повторите пароль")]
        public string ConfirmPassword { get; set; } = "";
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введите логин")]
        [Display(Name = "Логин")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = "";

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }
}
