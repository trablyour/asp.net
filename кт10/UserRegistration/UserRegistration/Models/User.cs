namespace UserRegistration.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";

        // пароль храним только в виде хэша
        public string PasswordHash { get; set; } = "";

        public DateTime CreatedAt { get; set; }
    }
}
