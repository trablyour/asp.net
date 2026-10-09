using Microsoft.AspNetCore.Mvc;
using EchoApp.Extensions;

namespace EchoApp.Controllers
{
    public class EchoController : Controller
    {
        // /Echo/Get - только GET
        [HttpGet]
        public async Task Get()
        {
            Response.ContentType = "text/plain";
            await Response.WriteAsync("GET request received");
        }

        // /Echo/Post - только POST
        [HttpPost]
        public async Task Post()
        {
            Response.ContentType = "text/plain";
            await Response.WriteAsync("POST request received");
        }

        // /Echo/Headers - все заголовки запроса
        public async Task Headers()
        {
            var headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            Response.ContentType = "application/json";
            await Response.WriteAsJsonAsync(headers);
        }

        // /Echo/Query?name=Tom&age=25 - параметры строки запроса
        public async Task Query()
        {
            var query = Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());

            Response.ContentType = "application/json";
            await Response.WriteAsJsonAsync(query);
        }

        // /Echo/Body - возвращает тело запроса
        public async Task Body()
        {
            string body = await Request.ReadAsStringAsync();

            Response.ContentType = "text/plain";
            await Response.WriteAsync(body);
        }
    }
}
