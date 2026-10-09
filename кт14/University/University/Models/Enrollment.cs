using System.ComponentModel.DataAnnotations.Schema;

namespace University.Models
{
    // промежуточная таблица для связи многие ко многим Student <-> Course
    [Table("Enrollments")]
    public class Enrollment
    {
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        // дата записи на курс, ради нее связь сделана через явную сущность
        public DateTime EnrolledAt { get; set; }
    }
}
