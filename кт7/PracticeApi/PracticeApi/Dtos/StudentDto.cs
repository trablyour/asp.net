using System.ComponentModel.DataAnnotations;

namespace PracticeApi.Dtos
{
    public class StudentDto
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = "";

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = "";

        [Required]
        [MaxLength(20)]
        public string Group { get; set; } = "";

        [Range(1, 6)]
        public int Course { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
    }
}
