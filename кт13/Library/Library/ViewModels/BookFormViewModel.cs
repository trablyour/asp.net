using System.ComponentModel.DataAnnotations;

namespace Library.ViewModels
{
    public class BookFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название")]
        [MaxLength(200)]
        [Display(Name = "Название")]
        public string Title { get; set; } = "";

        [Range(1, 2100, ErrorMessage = "Некорректный год")]
        [Display(Name = "Год издания")]
        public int Year { get; set; } = DateTime.Now.Year;

        [MaxLength(50)]
        [Display(Name = "Жанр")]
        public string? Genre { get; set; }

        [Range(1, 10000, ErrorMessage = "От 1 до 10000 страниц")]
        [Display(Name = "Страниц")]
        public int? Pages { get; set; }

        [MaxLength(20)]
        [RegularExpression(@"^[0-9\-]+$", ErrorMessage = "ISBN может содержать только цифры и дефисы")]
        [Display(Name = "ISBN")]
        public string? Isbn { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Выберите автора")]
        [Display(Name = "Автор")]
        public int AuthorId { get; set; }
    }
}
