using System.ComponentModel.DataAnnotations;
using UserRegistration.Models;

namespace UserRegistration.Dtos
{
    // что приходит при POST
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "Поле username обязательно")]
        [MinLength(3, ErrorMessage = "Имя пользователя должно быть не короче 3 символов")]
        [MaxLength(30, ErrorMessage = "Имя пользователя должно быть не длиннее 30 символов")]
        [RegularExpression(ValidationRules.UsernamePattern, ErrorMessage = ValidationRules.UsernameMessage)]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Поле email обязательно")]
        [RegularExpression(ValidationRules.EmailPattern, ErrorMessage = ValidationRules.EmailMessage)]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Поле password обязательно")]
        [MinLength(8, ErrorMessage = "Пароль должен быть не короче 8 символов")]
        [RegularExpression(ValidationRules.PasswordPattern, ErrorMessage = ValidationRules.PasswordMessage)]
        public string Password { get; set; } = "";
    }

    // что приходит при PUT, пароль можно не передавать
    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "Поле username обязательно")]
        [MinLength(3, ErrorMessage = "Имя пользователя должно быть не короче 3 символов")]
        [MaxLength(30, ErrorMessage = "Имя пользователя должно быть не длиннее 30 символов")]
        [RegularExpression(ValidationRules.UsernamePattern, ErrorMessage = ValidationRules.UsernameMessage)]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Поле email обязательно")]
        [RegularExpression(ValidationRules.EmailPattern, ErrorMessage = ValidationRules.EmailMessage)]
        public string Email { get; set; } = "";

        [MinLength(8, ErrorMessage = "Пароль должен быть не короче 8 символов")]
        [RegularExpression(ValidationRules.PasswordPattern, ErrorMessage = ValidationRules.PasswordMessage)]
        public string? Password { get; set; }
    }

    // что отдаем клиенту (без хэша пароля)
    public class UserResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public DateTime CreatedAt { get; set; }

        public static UserResponse From(User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
