using System.ComponentModel.DataAnnotations;

namespace JwtChat.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Username { get; set; } = "";

        [Required]
        public string PasswordHash { get; set; } = "";

        // "User" или "Admin" (первый зарегистрированный становится админом)
        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = Roles.User;

        public DateTime CreatedAt { get; set; }
    }

    public static class Roles
    {
        public const string User = "User";
        public const string Admin = "Admin";
    }
}
