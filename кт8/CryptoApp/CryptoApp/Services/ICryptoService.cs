namespace CryptoApp.Services
{
    public interface ICryptoService
    {
        string GenerateAesKey();
        string EncryptAes(string plainText, string keyBase64);
        string DecryptAes(string cipherBase64, string keyBase64);

        (string PublicKey, string PrivateKey) GenerateRsaKeys();
        string EncryptRsa(string plainText, string publicKeyPem);
        string DecryptRsa(string cipherBase64, string privateKeyPem);
    }
}
