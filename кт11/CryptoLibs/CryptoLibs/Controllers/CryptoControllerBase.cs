using Microsoft.AspNetCore.Mvc;
using CryptoLibs.Models;
using CryptoLibs.Services;

namespace CryptoLibs.Controllers
{
    // общая логика страницы шифрования, наследники только передают свою библиотеку
    public abstract class CryptoControllerBase : Controller
    {
        protected abstract ICryptoProvider Provider { get; }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new CryptoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CryptoViewModel model, string command)
        {
            ModelState.Clear();

            try
            {
                switch (command)
                {
                    case "generate":
                        model.Key = Provider.GenerateKey();
                        model.CipherText = null;
                        model.DecryptedText = null;
                        model.Message = "Сгенерирован новый ключ (256 бит).";
                        break;

                    case "encrypt":
                        Encrypt(model);
                        break;

                    case "decrypt":
                        Decrypt(model);
                        break;
                }
            }
            catch (CryptoOperationException ex)
            {
                model.Error = ex.Message;
            }

            return View(model);
        }

        private void Encrypt(CryptoViewModel model)
        {
            if (string.IsNullOrEmpty(model.PlainText))
            {
                model.Error = "Введите текст для шифрования.";
                return;
            }

            string? note = null;
            if (string.IsNullOrWhiteSpace(model.Key))
            {
                model.Key = Provider.GenerateKey();
                note = " Ключ не был указан, поэтому сгенерирован автоматически.";
            }

            model.CipherText = Provider.Encrypt(model.PlainText, model.Key);
            model.DecryptedText = null;
            model.Message = "Текст зашифрован." + note;
        }

        private void Decrypt(CryptoViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.CipherText))
            {
                model.Error = "Нет текста для расшифровки.";
                return;
            }

            if (string.IsNullOrWhiteSpace(model.Key))
            {
                model.Error = "Для расшифровки нужен ключ.";
                return;
            }

            model.DecryptedText = Provider.Decrypt(model.CipherText, model.Key);

            if (string.IsNullOrEmpty(model.PlainText))
                model.Message = "Текст расшифрован.";
            else if (model.DecryptedText == model.PlainText)
                model.Message = "Расшифровано. Результат совпадает с исходным текстом.";
            else
                model.Message = "Расшифровано, но результат не совпадает с исходным текстом.";
        }
    }
}
