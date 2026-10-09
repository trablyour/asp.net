using System.ComponentModel.DataAnnotations;
using UserRegistration.Models;

namespace UserRegistration.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите имя пользователя")]
        [MinLength(3, ErrorMessage = "Имя пользователя должно быть не короче 3 символов")]
        [MaxLength(30, ErrorMessage = "Имя пользователя должно быть не длиннее 30 символов")]
        [RegularExpression(ValidationRules.UsernamePattern, ErrorMessage = ValidationRules.UsernameMessage)]
        [Display(Name = "Имя пользователя")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Введите email")]
        [RegularExpression(ValidationRules.EmailPattern, ErrorMessage = ValidationRules.EmailMessage)]
        [Display(Name = "Электронная почта")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Введите пароль")]
        [MinLength(8, ErrorMessage = "Пароль должен быть не короче 8 символов")]
        [RegularExpression(ValidationRules.PasswordPattern, ErrorMessage = ValidationRules.PasswordMessage)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Подтвердите пароль")]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        public string ConfirmPassword { get; set; } = "";
    }
}
