using System.ComponentModel.DataAnnotations;

namespace SecureChat.Models
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

    // то, что уходит клиентам через SignalR
    public record MessageDto(string UserName, string Text, DateTime SentAt);
}
