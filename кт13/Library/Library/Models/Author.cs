namespace Library.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public int? BirthYear { get; set; }
        public string? Country { get; set; }
        public string? Biography { get; set; }

        // один автор - много книг
        public List<Book> Books { get; set; } = new();

        public string FullName => $"{FirstName} {LastName}";
    }
}
