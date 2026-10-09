using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using CryptoApp.Models;
using CryptoApp.Services;

namespace CryptoApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICryptoService _crypto;

        public HomeController(ICryptoService crypto)
        {
            _crypto = crypto;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new CryptoViewModel());
        }

        // command приходит из нажатой кнопки: generate, encrypt или decrypt
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CryptoViewModel model, string command)
        {
            // иначе форма покажет старые значения вместо новых из модели
            ModelState.Clear();

            try
            {
                switch (command)
                {
                    case "generate":
                        GenerateKeys(model);
                        break;
                    case "encrypt":
                        Encrypt(model);
                        break;
                    case "decrypt":
                        Decrypt(model);
                        break;
                }
            }
            catch (FormatException)
            {
                model.Error = "Ключ или зашифрованный текст не являются корректной строкой Base64.";
            }
            catch (CryptographicException)
            {
                model.Error = "Ошибка шифрования: неверный ключ или поврежденные данные.";
            }
            catch (ArgumentException)
            {
                model.Error = "Неверный формат ключа.";
            }
            catch (InvalidOperationException ex)
            {
                model.Error = ex.Message;
            }

            return View(model);
        }

        private void GenerateKeys(CryptoViewModel model)
        {
            if (model.Algorithm == "RSA")
            {
                var keys = _crypto.GenerateRsaKeys();
                model.RsaPublicKey = keys.PublicKey;
                model.RsaPrivateKey = keys.PrivateKey;
                model.Message = "Сгенерирована новая пара ключей RSA (2048 бит).";
            }
            else
            {
                model.AesKey = _crypto.GenerateAesKey();
                model.Message = "Сгенерирован новый ключ AES (256 бит).";
            }

            model.CipherText = null;
            model.DecryptedText = null;
        }

        private void Encrypt(CryptoViewModel model)
        {
            if (string.IsNullOrEmpty(model.PlainText))
            {
                model.Error = "Введите текст для шифрования.";
                return;
            }

            string? autoKeyMessage = null;

            if (model.Algorithm == "RSA")
            {
                if (string.IsNullOrWhiteSpace(model.RsaPublicKey))
                {
                    var keys = _crypto.GenerateRsaKeys();
                    model.RsaPublicKey = keys.PublicKey;
                    model.RsaPrivateKey = keys.PrivateKey;
                    autoKeyMessage = " Ключи не были указаны, поэтому сгенерированы автоматически.";
                }

                model.CipherText = _crypto.EncryptRsa(model.PlainText, model.RsaPublicKey);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.AesKey))
                {
                    model.AesKey = _crypto.GenerateAesKey();
                    autoKeyMessage = " Ключ не был указан, поэтому сгенерирован автоматически.";
                }

                model.CipherText = _crypto.EncryptAes(model.PlainText, model.AesKey.Trim());
            }

            model.DecryptedText = null;
            model.Message = "Текст зашифрован." + autoKeyMessage;
        }

        private void Decrypt(CryptoViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.CipherText))
            {
                model.Error = "Нет текста для расшифровки.";
                return;
            }

            if (model.Algorithm == "RSA")
            {
                if (string.IsNullOrWhiteSpace(model.RsaPrivateKey))
                {
                    model.Error = "Для расшифровки RSA нужен закрытый ключ.";
                    return;
                }

                model.DecryptedText = _crypto.DecryptRsa(model.CipherText.Trim(), model.RsaPrivateKey);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.AesKey))
                {
                    model.Error = "Для расшифровки AES нужен ключ.";
                    return;
                }

                model.DecryptedText = _crypto.DecryptAes(model.CipherText.Trim(), model.AesKey.Trim());
            }

            // проверка корректности: сравниваем с исходным текстом
            if (!string.IsNullOrEmpty(model.PlainText))
            {
                model.Message = model.DecryptedText == model.PlainText
                    ? "Расшифровано. Результат совпадает с исходным текстом."
                    : "Расшифровано, но результат НЕ совпадает с исходным текстом.";
            }
            else
            {
                model.Message = "Текст расшифрован.";
            }
        }
    }
}
