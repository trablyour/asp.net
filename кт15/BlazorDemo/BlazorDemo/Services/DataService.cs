using BlazorDemo.Models;

namespace BlazorDemo.Services
{
    // тестовые данные для страниц товаров и услуг
    public class DataService
    {
        public string[] Categories { get; } = { "phones", "laptops", "accessories" };

        private readonly List<Product> _products = new()
        {
            new Product { Id = 1, NameRu = "Смартфон Nova 12", NameEn = "Nova 12 smartphone", Category = "phones", Price = 29990,
                DescriptionRu = "6.5 дюйма, 128 ГБ, камера 50 Мп", DescriptionEn = "6.5 inch, 128 GB, 50 MP camera" },
            new Product { Id = 2, NameRu = "Смартфон Lite 8", NameEn = "Lite 8 smartphone", Category = "phones", Price = 14990,
                DescriptionRu = "Компактный и недорогой", DescriptionEn = "Compact and affordable" },
            new Product { Id = 3, NameRu = "Ноутбук Pro 15", NameEn = "Pro 15 laptop", Category = "laptops", Price = 89990,
                DescriptionRu = "16 ГБ ОЗУ, SSD 512 ГБ", DescriptionEn = "16 GB RAM, 512 GB SSD" },
            new Product { Id = 4, NameRu = "Ноутбук Air 13", NameEn = "Air 13 laptop", Category = "laptops", Price = 64990,
                DescriptionRu = "Легкий, до 12 часов работы", DescriptionEn = "Lightweight, up to 12 hours battery" },
            new Product { Id = 5, NameRu = "Беспроводные наушники", NameEn = "Wireless headphones", Category = "accessories", Price = 4990,
                DescriptionRu = "Шумоподавление, 30 часов", DescriptionEn = "Noise cancelling, 30 hours" },
            new Product { Id = 6, NameRu = "Чехол для телефона", NameEn = "Phone case", Category = "accessories", Price = 990,
                DescriptionRu = "Силикон, разные цвета", DescriptionEn = "Silicone, various colors" }
        };

        private readonly List<ServiceItem> _services = new()
        {
            new ServiceItem { Id = 1, NameRu = "Замена экрана", NameEn = "Screen replacement", DurationDays = 1, Price = 3500 },
            new ServiceItem { Id = 2, NameRu = "Чистка ноутбука", NameEn = "Laptop cleaning", DurationDays = 2, Price = 2000 },
            new ServiceItem { Id = 3, NameRu = "Установка ПО", NameEn = "Software installation", DurationDays = 1, Price = 1500 },
            new ServiceItem { Id = 4, NameRu = "Восстановление данных", NameEn = "Data recovery", DurationDays = 5, Price = 7000 }
        };

        public List<Product> GetProducts(string? category = null)
        {
            if (string.IsNullOrEmpty(category))
                return _products;

            return _products.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public Product? GetProduct(int id) => _products.FirstOrDefault(p => p.Id == id);

        public int MaxProductId => _products.Max(p => p.Id);

        public List<ServiceItem> GetServices() => _services;

        public bool CategoryExists(string category) =>
            Categories.Contains(category, StringComparer.OrdinalIgnoreCase);
    }
}
