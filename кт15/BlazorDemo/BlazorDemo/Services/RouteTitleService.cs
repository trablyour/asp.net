namespace BlazorDemo.Services
{
    // заголовок страницы по текущему маршруту
    public class RouteTitleService
    {
        private readonly LocalizationService _l;

        public RouteTitleService(LocalizationService l)
        {
            _l = l;
        }

        public string GetTitle(string path)
        {
            string clean = path.Split('?', '#')[0];
            string[] parts = clean.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                return _l["title_home"];

            switch (parts[0].ToLower())
            {
                case "products":
                    if (parts.Length == 1)
                        return _l["title_products"];
                    string key = "cat_" + parts[1].ToLower();
                    return $"{_l["title_products"]}: {(_l.Has(key) ? _l[key] : parts[1])}";

                case "details":
                    if (parts.Length == 2 && int.TryParse(parts[1], out int id))
                        return $"{_l["title_details"]} #{id}";
                    return _l["title_notfound"];

                case "catalog":
                    if (parts.Length == 1)
                        return _l["title_catalog"];
                    string section = "section_" + parts[1].ToLower();
                    return $"{_l["title_catalog"]}: {(_l.Has(section) ? _l[section] : parts[1])}";

                case "error-test":
                    return _l["title_error"];

                default:
                    return _l["title_notfound"];
            }
        }
    }
}
