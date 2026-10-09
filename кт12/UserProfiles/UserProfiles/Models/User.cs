using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserProfiles.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = "";

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        // навигационное свойство: у пользователя один профиль (может отсутствовать)
        public UserProfile? Profile { get; set; }
    }
}
