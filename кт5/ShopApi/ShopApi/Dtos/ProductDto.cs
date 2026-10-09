using System.ComponentModel.DataAnnotations;

namespace ShopApi.Dtos
{
    // то, что приходит от клиента при создании/изменении товара
    public class ProductDto
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = "";

        [MaxLength(500)]
        public string? Description { get; set; }

        [Range(typeof(decimal), "0", "1000000")]
        public decimal Price { get; set; }

        [Range(0, 100000)]
        public int Stock { get; set; }

        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
    }
}
