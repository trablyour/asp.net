using SalesCharts.Models;

namespace SalesCharts.Services
{
    // генерирует правдоподобные тестовые продажи
    public static class SalesDataGenerator
    {
        public static readonly string[] Regions = { "Все регионы", "Москва", "Санкт-Петербург", "Казань", "Новосибирск" };
        private static readonly double[] RegionShare = { 1.0, 0.42, 0.26, 0.17, 0.15 };

        public static readonly string[] Products = { "Ноутбуки", "Телефоны", "Планшеты", "Аксессуары", "Наушники" };
        private static readonly double[] ProductWeight = { 0.34, 0.30, 0.14, 0.12, 0.10 };

        private static readonly string[] Days = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };
        private static readonly string[] Months = { "Янв", "Фев", "Мар", "Апр", "Май", "Июн", "Июл", "Авг", "Сен", "Окт", "Ноя", "Дек" };

        // сезонность по месяцам: летом спад, к Новому году рост
        private static readonly double[] Season = { 0.85, 0.8, 0.9, 0.95, 1.0, 0.9, 0.85, 0.9, 1.05, 1.1, 1.3, 1.6 };

        public static List<SalesPoint> Generate(SalesPeriod period, int regionIndex, int seed, bool previous = false)
        {
            var rnd = new Random(seed * 31 + (int)period * 7 + regionIndex * 13 + (previous ? 1000 : 0));
            double share = RegionShare[Math.Clamp(regionIndex, 0, RegionShare.Length - 1)];
            double prevFactor = previous ? 0.85 : 1.0;

            var result = new List<SalesPoint>();

            switch (period)
            {
                case SalesPeriod.Week:
                    for (int i = 0; i < 7; i++)
                    {
                        double weekend = i >= 5 ? 1.35 : 1.0;
                        result.Add(Point(Days[i], 450_000 * share * weekend * prevFactor * Noise(rnd)));
                    }
                    break;

                case SalesPeriod.Month:
                    for (int i = 1; i <= 30; i++)
                    {
                        double trend = 1 + i * 0.008;
                        double weekend = (i % 7 == 6 || i % 7 == 0) ? 1.3 : 1.0;
                        result.Add(Point(i.ToString(), 400_000 * share * trend * weekend * prevFactor * Noise(rnd)));
                    }
                    break;

                case SalesPeriod.Year:
                    for (int i = 0; i < 12; i++)
                    {
                        double trend = 1 + i * 0.02;
                        result.Add(Point(Months[i], 12_000_000 * share * Season[i] * trend * prevFactor * Noise(rnd)));
                    }
                    break;
            }

            return result;
        }

        public static List<SalesPoint> ByProduct(SalesPeriod period, int seed)
        {
            var rnd = new Random(seed * 17 + (int)period);
            decimal total = Generate(period, 0, seed).Sum(p => p.Value);

            var result = new List<SalesPoint>();
            for (int i = 0; i < Products.Length; i++)
            {
                double weight = ProductWeight[i] * (0.85 + rnd.NextDouble() * 0.3);
                result.Add(Point(Products[i], (double)total * weight));
            }

            return result.OrderByDescending(p => p.Value).ToList();
        }

        // первые точки для онлайн-режима
        public static List<SalesPoint> LiveStart(Random rnd, int count)
        {
            var result = new List<SalesPoint>();
            decimal value = 50_000;
            var time = DateTime.Now.AddSeconds(-2 * count);

            for (int i = 0; i < count; i++)
            {
                value = NextLiveValue(rnd, value);
                time = time.AddSeconds(2);
                result.Add(new SalesPoint { Label = time.ToString("HH:mm:ss"), Value = value });
            }

            return result;
        }

        // случайное блуждание: следующее значение рядом с предыдущим
        public static decimal NextLiveValue(Random rnd, decimal last)
        {
            double change = (rnd.NextDouble() - 0.48) * 0.25;
            decimal next = last * (decimal)(1 + change);
            return Math.Round(Math.Clamp(next, 15_000m, 120_000m) / 100) * 100;
        }

        private static double Noise(Random rnd) => 0.8 + rnd.NextDouble() * 0.4;

        private static SalesPoint Point(string label, double value) =>
            new SalesPoint { Label = label, Value = Math.Round((decimal)value / 100) * 100 };
    }
}
