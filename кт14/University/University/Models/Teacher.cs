using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University.Models
{
    [Table("Teachers")]
    public class Teacher
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = "";

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = "";

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = "";

        [MaxLength(100)]
        public string? Department { get; set; }

        // один преподаватель - много курсов
        public List<Course> Courses { get; set; } = new();

        [NotMapped]
        public string FullName => $"{LastName} {FirstName}";
    }
}
