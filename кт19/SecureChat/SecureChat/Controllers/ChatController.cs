using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SecureChat.Controllers
{
    // неавторизованного пользователя перекинет на /Account/Login
    [Authorize]
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
