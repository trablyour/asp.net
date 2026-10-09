namespace BlazorEvents.Models
{
    public class FeedbackMessage
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Text { get; set; } = "";
        public DateTime SentAt { get; set; }
    }
}
