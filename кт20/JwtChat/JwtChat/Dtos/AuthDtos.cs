using System.ComponentModel.DataAnnotations;

namespace JwtChat.Dtos
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Введите логин")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Логин от 3 до 20 символов")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Логин: только латинские буквы, цифры и _")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Введите пароль")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль не короче 6 символов")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).+$", ErrorMessage = "Пароль должен содержать буквы и цифры")]
        public string Password { get; set; } = "";
    }

    public class LoginRequest
    {
        [Required(ErrorMessage = "Введите логин")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Введите пароль")]
        public string Password { get; set; } = "";
    }

    public record AuthResponse(string Token, string UserName, string Role, DateTime ExpiresAt);
}
