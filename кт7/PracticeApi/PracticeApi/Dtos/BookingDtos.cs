using System.ComponentModel.DataAnnotations;

namespace PracticeApi.Dtos
{
    public class CreateBookingDto
    {
        [Range(1, int.MaxValue)]
        public int ResourceId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; } = "";

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class UpdateBookingDto
    {
        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; } = "";

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
