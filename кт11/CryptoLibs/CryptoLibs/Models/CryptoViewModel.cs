namespace CryptoLibs.Models
{
    public class CryptoViewModel
    {
        public string? Key { get; set; }
        public string? PlainText { get; set; }
        public string? CipherText { get; set; }
        public string? DecryptedText { get; set; }

        public string? Message { get; set; }
        public string? Error { get; set; }
    }
}
