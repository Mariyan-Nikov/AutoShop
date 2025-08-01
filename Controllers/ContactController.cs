using Microsoft.AspNetCore.Mvc;

namespace AutoShop.Controllers
{
    public class ContactController : Controller
    {
        // Действие за показване на контактната страница (Views/Contact/Index.cshtml)
        public IActionResult Index()
        {
            return View();
        }
    }
}
