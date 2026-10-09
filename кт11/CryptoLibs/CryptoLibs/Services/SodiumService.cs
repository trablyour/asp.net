using System.Security.Cryptography;
using System.Text;
using Sodium;

namespace CryptoLibs.Services
{
    // Задание 2. libsodium: crypto_secretbox (XSalsa20 + Poly1305)
    public class SodiumService : ICryptoProvider
    {
        private const int KeySize = 32;
        private const int NonceSize = 24;

        public string GenerateKey()
        {
            return Convert.ToBase64String(SecretBox.GenerateKey());
        }

        // результат: nonce (24 байта) + шифротекст с MAC, в Base64
        public string Encrypt(string plainText, string keyBase64)
        {
            byte[] key = ParseKey(keyBase64);
            byte[] nonce = SecretBox.GenerateNonce();

            byte[] encrypted = SecretBox.Create(Encoding.UTF8.GetBytes(plainText), nonce, key);

            byte[] result = new byte[nonce.Length + encrypted.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
            Buffer.BlockCopy(encrypted, 0, result, nonce.Length, encrypted.Length);

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string cipherBase64, string keyBase64)
        {
            byte[] key = ParseKey(keyBase64);
            byte[] data = ParseBase64(cipherBase64, "Зашифрованный текст");

            if (data.Length <= NonceSize)
                throw new CryptoOperationException("Зашифрованные данные слишком короткие");

            byte[] nonce = data[..NonceSize];
            byte[] encrypted = data[NonceSize..];

            try
            {
                byte[] decrypted = SecretBox.Open(encrypted, nonce, key);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch (CryptographicException)
            {
                // Poly1305 не сошелся: ключ не тот или данные изменены
                throw new CryptoOperationException("Не удалось расшифровать: неверный ключ или данные были изменены");
            }
        }

        private static byte[] ParseKey(string keyBase64)
        {
            byte[] key = ParseBase64(keyBase64, "Ключ");
            if (key.Length != KeySize)
                throw new CryptoOperationException($"Ключ libsodium должен быть ровно {KeySize} байта, а сейчас {key.Length}");
            return key;
        }

        private static byte[] ParseBase64(string value, string fieldName)
        {
            try
            {
                return Convert.FromBase64String(value.Trim());
            }
            catch (FormatException)
            {
                throw new CryptoOperationException($"{fieldName}: некорректная строка Base64");
            }
        }
    }
}
