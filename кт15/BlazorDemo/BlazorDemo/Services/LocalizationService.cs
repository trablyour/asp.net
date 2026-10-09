namespace BlazorDemo.Services
{
    // переключение языка без перезагрузки: меняем язык и сообщаем компонентам событием Changed
    public class LocalizationService
    {
        public string Language { get; private set; } = "ru";
        public string[] Languages { get; } = { "ru", "en" };

        public event Action? Changed;

        public void SetLanguage(string language)
        {
            if (language == Language || !Texts.ContainsKey(language))
                return;

            Language = language;
            Changed?.Invoke();
        }

        public string this[string key] =>
            Texts[Language].TryGetValue(key, out var value) ? value : key;

        public bool Has(string key) => Texts[Language].ContainsKey(key);

        // для данных, у которых есть поля на двух языках
        public string Pick(string ru, string en) => Language == "en" ? en : ru;

        public string Money(decimal value) => Language == "en" ? $"{value:N0} RUB" : $"{value:N0} ₽";

        private static readonly Dictionary<string, Dictionary<string, string>> Texts = new()
        {
            ["ru"] = new()
            {
                ["app_name"] = "Blazor Demo",
                ["menu_home"] = "Главная (кликер)",
                ["menu_products"] = "Товары",
                ["menu_catalog_products"] = "Каталог: товары",
                ["menu_catalog_services"] = "Каталог: услуги",
                ["menu_details"] = "Детали товара #1",
                ["menu_error"] = "Тест ошибки",
                ["menu_broken"] = "Битая ссылка",

                ["title_home"] = "Главная",
                ["title_products"] = "Товары",
                ["title_details"] = "Товар",
                ["title_catalog"] = "Каталог",
                ["title_error"] = "Тест ошибки",
                ["title_notfound"] = "Страница не найдена",

                ["current_route"] = "Текущий маршрут",
                ["toast_navigated"] = "Переход на",
                ["language"] = "Язык",

                ["clicker_title"] = "Часть 1. Кликер",
                ["clicker_text"] = "Компонент StudentComponent получает имя и возраст через параметры.",
                ["age"] = "Возраст",
                ["initial_age"] = "Начальный возраст",
                ["add_year"] = "+1 год",
                ["reset"] = "Сбросить",
                ["go_products"] = "Перейти к товарам",

                ["all_categories"] = "Все",
                ["category"] = "Категория",
                ["showing_all"] = "Параметр category не передан, показаны все товары",
                ["unknown_category"] = "Такой категории нет",
                ["cat_phones"] = "Телефоны",
                ["cat_laptops"] = "Ноутбуки",
                ["cat_accessories"] = "Аксессуары",
                ["open"] = "Подробнее",
                ["go_catalog"] = "Перейти в каталог",

                ["details_lifecycle"] = "Компонент не пересоздается при смене id: данные подгружаются в OnParametersSet",
                ["init_calls"] = "Вызовов OnInitialized",
                ["params_calls"] = "Вызовов OnParametersSet",
                ["prev"] = "← Предыдущий",
                ["next"] = "Следующий →",
                ["back_to_category"] = "Назад к категории",
                ["product_not_found"] = "Товар с таким id не найден",
                ["price"] = "Цена",

                ["catalog_text"] = "Компонент выбирается по параметру маршрута через DynamicComponent",
                ["catalog_choose"] = "Выберите раздел",
                ["section_products"] = "Товары",
                ["section_services"] = "Услуги",
                ["unknown_section"] = "Неизвестный раздел",
                ["service"] = "Услуга",
                ["duration"] = "Срок",
                ["days"] = "дн.",
                ["go_home"] = "На главную",

                ["error_text"] = "Кнопка ниже выбрасывает исключение. Его перехватит ErrorBoundary в MainLayout.",
                ["throw"] = "Выбросить исключение",
                ["error_message"] = "Тестовая ошибка в компоненте",
                ["error_title"] = "В компоненте произошла ошибка",
                ["error_recover"] = "Попробовать снова",

                ["notfound_text"] = "По адресу ниже ничего нет:",
                ["notfound_hint"] = "Проверьте адрес или вернитесь на главную."
            },
            ["en"] = new()
            {
                ["app_name"] = "Blazor Demo",
                ["menu_home"] = "Home (clicker)",
                ["menu_products"] = "Products",
                ["menu_catalog_products"] = "Catalog: products",
                ["menu_catalog_services"] = "Catalog: services",
                ["menu_details"] = "Product details #1",
                ["menu_error"] = "Error test",
                ["menu_broken"] = "Broken link",

                ["title_home"] = "Home",
                ["title_products"] = "Products",
                ["title_details"] = "Product",
                ["title_catalog"] = "Catalog",
                ["title_error"] = "Error test",
                ["title_notfound"] = "Page not found",

                ["current_route"] = "Current route",
                ["toast_navigated"] = "Navigated to",
                ["language"] = "Language",

                ["clicker_title"] = "Part 1. Clicker",
                ["clicker_text"] = "StudentComponent receives name and age as parameters.",
                ["age"] = "Age",
                ["initial_age"] = "Initial age",
                ["add_year"] = "+1 year",
                ["reset"] = "Reset",
                ["go_products"] = "Go to products",

                ["all_categories"] = "All",
                ["category"] = "Category",
                ["showing_all"] = "No category parameter, showing all products",
                ["unknown_category"] = "Unknown category",
                ["cat_phones"] = "Phones",
                ["cat_laptops"] = "Laptops",
                ["cat_accessories"] = "Accessories",
                ["open"] = "Details",
                ["go_catalog"] = "Go to catalog",

                ["details_lifecycle"] = "The component is not recreated when id changes: data is loaded in OnParametersSet",
                ["init_calls"] = "OnInitialized calls",
                ["params_calls"] = "OnParametersSet calls",
                ["prev"] = "← Previous",
                ["next"] = "Next →",
                ["back_to_category"] = "Back to category",
                ["product_not_found"] = "Product with this id was not found",
                ["price"] = "Price",

                ["catalog_text"] = "The component is chosen by the route parameter using DynamicComponent",
                ["catalog_choose"] = "Choose a section",
                ["section_products"] = "Products",
                ["section_services"] = "Services",
                ["unknown_section"] = "Unknown section",
                ["service"] = "Service",
                ["duration"] = "Duration",
                ["days"] = "days",
                ["go_home"] = "Go home",

                ["error_text"] = "The button below throws an exception. ErrorBoundary in MainLayout will catch it.",
                ["throw"] = "Throw exception",
                ["error_message"] = "Test error in component",
                ["error_title"] = "An error occurred in the component",
                ["error_recover"] = "Try again",

                ["notfound_text"] = "Nothing was found at this address:",
                ["notfound_hint"] = "Check the address or go back home."
            }
        };
    }
}
