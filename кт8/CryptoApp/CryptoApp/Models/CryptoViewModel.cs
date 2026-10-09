namespace CryptoApp.Models
{
    public class CryptoViewModel
    {
        // "AES" или "RSA"
        public string Algorithm { get; set; } = "AES";

        public string? PlainText { get; set; }
        public string? CipherText { get; set; }
        public string? DecryptedText { get; set; }

        // ключ AES в Base64
        public string? AesKey { get; set; }

        // ключи RSA в формате PEM
        public string? RsaPublicKey { get; set; }
        public string? RsaPrivateKey { get; set; }

        public string? Message { get; set; }
        public string? Error { get; set; }
    }
}
