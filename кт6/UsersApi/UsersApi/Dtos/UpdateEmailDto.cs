using System.ComponentModel.DataAnnotations;

namespace UsersApi.Dtos
{
    public class UpdateEmailDto
    {
        // текущее имя пользователя, email обновится только если оно совпадет
        [Required]
        public string CurrentName { get; set; } = "";

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string NewEmail { get; set; } = "";
    }
}
