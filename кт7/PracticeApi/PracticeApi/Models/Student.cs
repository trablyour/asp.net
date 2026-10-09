namespace PracticeApi.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Group { get; set; } = "";
        public int Course { get; set; }
        public string? Email { get; set; }
    }
}
