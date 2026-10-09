namespace Library.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public int Year { get; set; }
        public string? Genre { get; set; }
        public int? Pages { get; set; }
        public string? Isbn { get; set; }

        // у книги один автор
        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;
    }
}
