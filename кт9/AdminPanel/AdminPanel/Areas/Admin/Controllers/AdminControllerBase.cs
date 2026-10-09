using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminPanel.Models;

namespace AdminPanel.Areas.Admin.Controllers
{
    // все контроллеры админки наследуются отсюда и закрыты политикой AdminOnly
    [Area("Admin")]
    [Authorize(Policy = Policies.AdminOnly)]
    public abstract class AdminControllerBase : Controller
    {
    }
}
