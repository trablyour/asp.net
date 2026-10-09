using Microsoft.AspNetCore.Mvc;
using UsersApi.Services;

namespace UsersApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly UpdateLogger _log;

        public LogsController(UpdateLogger log)
        {
            _log = log;
        }

        // GET api/logs - посмотреть журнал обновлений
        [HttpGet]
        public IActionResult Get()
        {
            return Content(_log.ReadAll(), "text/plain; charset=utf-8");
        }
    }
}
