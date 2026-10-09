using System.ComponentModel.DataAnnotations;

namespace JwtChat.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        [MaxLength(20)]
        public string UserName { get; set; } = "";

        [Required]
        [MaxLength(500)]
        public string Text { get; set; } = "";

        public DateTime SentAt { get; set; }
    }

    public record MessageDto(string UserName, string Text, DateTime SentAt);
}
