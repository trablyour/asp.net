using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University.Models
{
    [Table("Students")]
    public class Student
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

        [Required]
        [MaxLength(20)]
        public string Group { get; set; } = "";

        // записи на курсы (многие ко многим через Enrollment)
        public List<Enrollment> Enrollments { get; set; } = new();

        [NotMapped]
        public string FullName => $"{LastName} {FirstName}";
    }
}
