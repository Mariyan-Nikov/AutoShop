using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")] // Само администратори имат достъп
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
