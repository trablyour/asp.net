using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace CryptoLibs.Services
{
    // Задание 1. AES-256 в режиме GCM через Bouncy Castle
    public class BouncyCastleAesService : ICryptoProvider
    {
        private const int KeySize = 32;    // 256 бит
        private const int NonceSize = 12;  // стандартный размер для GCM
        private const int TagSize = 128;   // тег аутентификации в битах

        private readonly SecureRandom _random = new SecureRandom();

        public string GenerateKey()
        {
            byte[] key = new byte[KeySize];
            _random.NextBytes(key);
            return Convert.ToBase64String(key);
        }

        // результат: nonce (12 байт) + шифротекст с тегом, в Base64
        public string Encrypt(string plainText, string keyBase64)
        {
            byte[] key = ParseKey(keyBase64);
            byte[] nonce = new byte[NonceSize];
            _random.NextBytes(nonce);

            byte[] input = Encoding.UTF8.GetBytes(plainText);

            var cipher = new GcmBlockCipher(new AesEngine());
            cipher.Init(true, new AeadParameters(new KeyParameter(key), TagSize, nonce));

            byte[] encrypted = new byte[cipher.GetOutputSize(input.Length)];
            int length = cipher.ProcessBytes(input, 0, input.Length, encrypted, 0);
            cipher.DoFinal(encrypted, length);

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

            var cipher = new GcmBlockCipher(new AesEngine());
            cipher.Init(false, new AeadParameters(new KeyParameter(key), TagSize, nonce));

            byte[] decrypted = new byte[cipher.GetOutputSize(encrypted.Length)];
            try
            {
                int length = cipher.ProcessBytes(encrypted, 0, encrypted.Length, decrypted, 0);
                length += cipher.DoFinal(decrypted, length);
                return Encoding.UTF8.GetString(decrypted, 0, length);
            }
            catch (InvalidCipherTextException)
            {
                // GCM проверяет тег: если ключ не тот или данные изменены, будет ошибка
                throw new CryptoOperationException("Не удалось расшифровать: неверный ключ или данные были изменены");
            }
        }

        private static byte[] ParseKey(string keyBase64)
        {
            byte[] key = ParseBase64(keyBase64, "Ключ");
            if (key.Length != 16 && key.Length != 24 && key.Length != 32)
                throw new CryptoOperationException($"Ключ AES должен быть 16, 24 или 32 байта, а сейчас {key.Length}");
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
