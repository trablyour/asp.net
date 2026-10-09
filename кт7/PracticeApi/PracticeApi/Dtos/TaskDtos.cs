using System.ComponentModel.DataAnnotations;
using PracticeApi.Models;

namespace PracticeApi.Dtos
{
    public class CreateTaskDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = "";

        [MaxLength(1000)]
        public string? Description { get; set; }
    }

    public class UpdateStatusDto
    {
        [Required]
        public TaskItemStatus? Status { get; set; }
    }
}
