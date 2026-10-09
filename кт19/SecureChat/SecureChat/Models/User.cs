using System.ComponentModel.DataAnnotations;

namespace SecureChat.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Username { get; set; } = "";

        // пароль храним только хэшем
        [Required]
        public string PasswordHash { get; set; } = "";

        public DateTime CreatedAt { get; set; }
    }
}
