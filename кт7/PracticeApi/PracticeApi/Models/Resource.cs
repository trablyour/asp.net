namespace PracticeApi.Models
{
    // базовый класс для всего, что можно забронировать
    public abstract class Resource
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
    }

    // номер в отеле
    public class HotelRoom : Resource
    {
        public int Beds { get; set; }
        public decimal PricePerNight { get; set; }
    }

    // столик в ресторане
    public class RestaurantTable : Resource
    {
        public int Seats { get; set; }
        public bool NearWindow { get; set; }
    }
}
