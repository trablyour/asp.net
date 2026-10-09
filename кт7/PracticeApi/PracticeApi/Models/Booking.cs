namespace PracticeApi.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public int ResourceId { get; set; }
        public Resource? Resource { get; set; }

        public string CustomerName { get; set; } = "";
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
