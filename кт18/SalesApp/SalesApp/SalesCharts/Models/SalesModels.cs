namespace SalesCharts.Models
{
    public class SalesPoint
    {
        public string Label { get; set; } = "";
        public decimal Value { get; set; }
    }

    public class SalesSeries
    {
        public string Name { get; set; } = "";
        public string Color { get; set; } = "#512bd4";
        public List<SalesPoint> Points { get; set; } = new();
    }

    public enum ChartType
    {
        Line,
        Area,
        Bar
    }

    public enum SalesPeriod
    {
        Week,
        Month,
        Year,
        Live
    }
}
