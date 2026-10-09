namespace UserRegistration.Models
{
    // общие правила для формы и для API
    public static class ValidationRules
    {
        // только буквы (латиница и кириллица)
        public const string UsernamePattern = @"^[a-zA-Zа-яА-ЯёЁ]+$";

        // есть @, после нее есть точка
        public const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        // хотя бы одна буква и хотя бы одна цифра
        public const string PasswordPattern = @"^(?=.*[a-zA-Zа-яА-ЯёЁ])(?=.*\d).+$";

        public const string UsernameMessage = "Имя пользователя может содержать только буквы";
        public const string EmailMessage = "Email должен содержать символы @ и .";
        public const string PasswordMessage = "Пароль должен содержать и буквы, и цифры";
    }
}
