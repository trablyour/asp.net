using System.Globalization;
using System.Text;

namespace SalesCharts.Services
{
    // функции для отрисовки графиков в SVG
    public static class ChartMath
    {
        private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

        // число для SVG-атрибута (всегда с точкой)
        public static string N(double value) => value.ToString("0.##", Inv);

        // "красивый" максимум оси: 100, 200, 250, 500, 1000...
        public static decimal NiceMax(decimal max)
        {
            if (max <= 0)
                return 10;

            double m = (double)max;
            double exp = Math.Pow(10, Math.Floor(Math.Log10(m)));
            double f = m / exp;

            double nice = f <= 1 ? 1 : f <= 2 ? 2 : f <= 2.5 ? 2.5 : f <= 5 ? 5 : 10;
            return (decimal)(nice * exp);
        }

        // ломаная линия через точки
        public static string LinePath(IReadOnlyList<(double X, double Y)> points)
        {
            if (points.Count == 0)
                return "";

            var sb = new StringBuilder();
            for (int i = 0; i < points.Count; i++)
            {
                sb.Append(i == 0 ? "M" : " L");
                sb.Append(N(points[i].X)).Append(' ').Append(N(points[i].Y));
            }
            return sb.ToString();
        }

        // та же линия, но замкнутая вниз до оси X (заливка)
        public static string AreaPath(IReadOnlyList<(double X, double Y)> points, double baseY)
        {
            if (points.Count == 0)
                return "";

            return LinePath(points)
                   + $" L{N(points[^1].X)} {N(baseY)}"
                   + $" L{N(points[0].X)} {N(baseY)} Z";
        }

        // сектор кольцевой диаграммы, углы в радианах
        public static string ArcPath(double cx, double cy, double rOuter, double rInner, double start, double end)
        {
            // полный круг одной дугой не нарисовать, чуть уменьшаем
            if (end - start >= 2 * Math.PI)
                end = start + 2 * Math.PI - 0.0001;

            int large = end - start > Math.PI ? 1 : 0;

            double x1 = cx + rOuter * Math.Cos(start), y1 = cy + rOuter * Math.Sin(start);
            double x2 = cx + rOuter * Math.Cos(end), y2 = cy + rOuter * Math.Sin(end);
            double x3 = cx + rInner * Math.Cos(end), y3 = cy + rInner * Math.Sin(end);
            double x4 = cx + rInner * Math.Cos(start), y4 = cy + rInner * Math.Sin(start);

            return $"M{N(x1)} {N(y1)} A{N(rOuter)} {N(rOuter)} 0 {large} 1 {N(x2)} {N(y2)} " +
                   $"L{N(x3)} {N(y3)} A{N(rInner)} {N(rInner)} 0 {large} 0 {N(x4)} {N(y4)} Z";
        }

        // 1 250 000 -> "1,3 млн", 15 000 -> "15 тыс"
        public static string Short(decimal value)
        {
            if (value >= 1_000_000)
                return (value / 1_000_000m).ToString("0.#", Inv).Replace('.', ',') + " млн";
            if (value >= 1_000)
                return (value / 1_000m).ToString("0.#", Inv).Replace('.', ',') + " тыс";
            return value.ToString("0", Inv);
        }

        // 1250000 -> "1 250 000 ₽"
        public static string Money(decimal value) =>
            value.ToString("#,0", Inv).Replace(",", " ") + " ₽";

        public static string Percent(double value) =>
            value.ToString("0.#", Inv).Replace('.', ',') + "%";
    }
}
