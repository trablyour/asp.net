using System.ComponentModel.DataAnnotations;

namespace ShopApi.Models
{
    public class Category
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = "";

        [MaxLength(500)]
        public string? Description { get; set; }

        public List<Product> Products { get; set; } = new();
    }
}
