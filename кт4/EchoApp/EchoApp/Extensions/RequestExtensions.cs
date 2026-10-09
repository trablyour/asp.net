using System.Text;

namespace EchoApp.Extensions
{
    public static class RequestExtensions
    {
        // у HttpRequest нет своего ReadAsStringAsync, поэтому делаем метод расширения
        public static async Task<string> ReadAsStringAsync(this HttpRequest request)
        {
            using (var reader = new StreamReader(request.Body, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
