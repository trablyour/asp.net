using System.ComponentModel.DataAnnotations;

namespace ShopApi.Dtos
{
    // то, что приходит от клиента при создании/изменении категории
    public class CategoryDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "";

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
