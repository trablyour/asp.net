using System.ComponentModel.DataAnnotations;

namespace UsersApi.Dtos
{
    public class UpdateUserDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "";

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = "";

        [Range(0, 150)]
        public int Age { get; set; }
    }
}
