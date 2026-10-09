using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ResponseApp.Models;

namespace ResponseApp.Controllers
{
    public class TestController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public TestController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // /Test/Text
        public async Task Text()
        {
            Response.ContentType = "text/plain";
            await Response.WriteAsync("Hello, world!");
        }

        // /Test/Html
        public async Task Html()
        {
            Response.ContentType = "text/html";
            await Response.WriteAsync("<h1>Hello from HTML</h1><p>This is a paragraph sent with WriteAsync.</p>");
        }

        // /Test/Json
        public async Task Json()
        {
            var person = new Person { Name = "Tom", Age = 25 };

            Response.ContentType = "application/json";

            // PropertyNamingPolicy = null чтобы в ответе было Name и Age, а не name и age
            var options = new JsonSerializerOptions { PropertyNamingPolicy = null };
            await Response.WriteAsJsonAsync(person, options);
        }

        // /Test/File
        public async Task File()
        {
            string path = Path.Combine(_env.ContentRootPath, "Files", "test.txt");

            Response.ContentType = "text/plain";
            await Response.SendFileAsync(path);
        }

        // /Test/Status
        public async Task Status()
        {
            Response.StatusCode = 404;
            Response.ContentType = "text/plain";
            await Response.WriteAsync("Error 404: page not found");
        }

        // /Test/Cookie
        public async Task Cookie()
        {
            Response.Cookies.Append("user", "Answer");

            Response.ContentType = "text/plain";
            await Response.WriteAsync("Cookie 'user' has been set");
        }
    }
}
