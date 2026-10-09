using System.Security.Cryptography;
using System.Text;

namespace CryptoApp.Services
{
    public class CryptoService : ICryptoService
    {
        // ---------- AES ----------

        // 256-битный ключ
        public string GenerateAesKey()
        {
            byte[] key = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(key);
        }

        // результат = IV (16 байт) + зашифрованные данные, все в Base64
        public string EncryptAes(string plainText, string keyBase64)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(keyBase64);
                aes.GenerateIV();

                byte[] data = Encoding.UTF8.GetBytes(plainText);
                byte[] encrypted = aes.EncryptCbc(data, aes.IV);

                byte[] result = new byte[aes.IV.Length + encrypted.Length];
                Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
                Buffer.BlockCopy(encrypted, 0, result, aes.IV.Length, encrypted.Length);

                return Convert.ToBase64String(result);
            }
        }

        public string DecryptAes(string cipherBase64, string keyBase64)
        {
            byte[] full = Convert.FromBase64String(cipherBase64);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(keyBase64);

                int ivLength = aes.BlockSize / 8;
                if (full.Length <= ivLength)
                    throw new CryptographicException("Слишком короткие данные");

                // отделяем IV от зашифрованных данных
                byte[] iv = full[..ivLength];
                byte[] encrypted = full[ivLength..];

                byte[] decrypted = aes.DecryptCbc(encrypted, iv);
                return Encoding.UTF8.GetString(decrypted);
            }
        }

        // ---------- RSA ----------

        public (string PublicKey, string PrivateKey) GenerateRsaKeys()
        {
            using (RSA rsa = RSA.Create(2048))
            {
                return (rsa.ExportRSAPublicKeyPem(), rsa.ExportRSAPrivateKeyPem());
            }
        }

        public string EncryptRsa(string plainText, string publicKeyPem)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportFromPem(publicKeyPem);

                byte[] data = Encoding.UTF8.GetBytes(plainText);

                // RSA шифрует только небольшие данные: для OAEP-SHA256 это размер ключа - 66 байт
                int maxLength = rsa.KeySize / 8 - 2 * 32 - 2;
                if (data.Length > maxLength)
                    throw new InvalidOperationException(
                        $"Текст слишком длинный для RSA: {data.Length} байт, максимум {maxLength}. Для больших текстов используйте AES.");

                byte[] encrypted = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
                return Convert.ToBase64String(encrypted);
            }
        }

        public string DecryptRsa(string cipherBase64, string privateKeyPem)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportFromPem(privateKeyPem);

                byte[] data = Convert.FromBase64String(cipherBase64);
                byte[] decrypted = rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA256);
                return Encoding.UTF8.GetString(decrypted);
            }
        }
    }
}
