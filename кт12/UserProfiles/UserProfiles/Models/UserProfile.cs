using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserProfiles.Models
{
    [Table("UserProfiles")]
    public class UserProfile
    {
        [Key]
        public int Id { get; set; }

        // внешний ключ на пользователя, уникальный (настроено в Fluent API)
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = "";

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = "";

        [Column(TypeName = "date")]
        public DateTime? BirthDate { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(500)]
        public string? Bio { get; set; }

        // навигационное свойство: профиль принадлежит одному пользователю
        public User User { get; set; } = null!;

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}
