using System.ComponentModel.DataAnnotations;

namespace ShopApi.Models
{
    public class Product
    {
        public int Id { get; set; }

        [MaxLength(150)]
        public string Name { get; set; } = "";

        [MaxLength(500)]
        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
