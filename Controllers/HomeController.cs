using System.Diagnostics;
using AutoShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace AutoShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var feature = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

            if (feature != null)
            {
                ViewBag.ErrorMessage = feature.Error.Message;
                ViewBag.Path = feature.Path;
            }
            else
            {
                ViewBag.ErrorMessage = "Неизвестна грешка";
                ViewBag.Path = "";
            }

            return View();
        }

        public IActionResult StatusCode(int code)
        {
            ViewBag.StatusCode = code;

            switch (code)
            {
                case 404:
                    ViewBag.Message = "Страницата не е намерена.";
                    break;
                case 500:
                    ViewBag.Message = "Вътрешна грешка на сървъра.";
                    break;
                default:
                    ViewBag.Message = "Възникна грешка.";
                    break;
            }

            return View();
        }
    }
}
