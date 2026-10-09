namespace CryptoLibs.Services
{
    // общий интерфейс, чтобы одна страница работала с любой библиотекой
    public interface ICryptoProvider
    {
        string GenerateKey();
        string Encrypt(string plainText, string keyBase64);
        string Decrypt(string cipherBase64, string keyBase64);
    }

    // ошибка с понятным текстом для пользователя
    public class CryptoOperationException : Exception
    {
        public CryptoOperationException(string message) : base(message)
        {
        }
    }
}
