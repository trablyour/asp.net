namespace BlazorDemo.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string NameRu { get; set; } = "";
        public string NameEn { get; set; } = "";
        public string DescriptionRu { get; set; } = "";
        public string DescriptionEn { get; set; } = "";
        public string Category { get; set; } = "";
        public decimal Price { get; set; }
    }

    public class ServiceItem
    {
        public int Id { get; set; }
        public string NameRu { get; set; } = "";
        public string NameEn { get; set; } = "";
        public int DurationDays { get; set; }
        public decimal Price { get; set; }
    }

    public class Toast
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Text { get; set; } = "";
    }
}
