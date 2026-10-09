using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University.Models
{
    [Table("Courses")]
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = "";

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Range(1, 20)]
        public int Credits { get; set; }

        // у курса один преподаватель
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;

        // студенты курса (многие ко многим через Enrollment)
        public List<Enrollment> Enrollments { get; set; } = new();
    }
}
