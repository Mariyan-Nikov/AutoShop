using System.Diagnostics;
using AutoShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace AutoShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // Конструктор за инжектиране на логер, който може да се използва за записване на събития и грешки
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Действие за началната страница
        public IActionResult Index()
        {
            return View();
        }

        // Действие за страницата с политика за поверителност
        public IActionResult Privacy()
        {
            return View();
        }

        // Действие за обработка на грешки
        // Атрибутите задават кеширането на отговора - в случая няма кеширане
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Взимаме информация за грешката от контекста на HTTP заявката
            var feature = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

            if (feature != null)
            {
                // Ако има грешка, подаваме съобщението и пътя към изгледа чрез ViewBag
                ViewBag.ErrorMessage = feature.Error.Message;
                ViewBag.Path = feature.Path;
            }
            else
            {
                // Ако няма конкретна грешка, задаваме общо съобщение
                ViewBag.ErrorMessage = "Неизвестна грешка";
                ViewBag.Path = "";
            }

            // Връщаме изгледа за грешки
            return View();
        }

        // Действие за статус кодове (например 404, 500 и др.)
        public IActionResult StatusCode(int code)
        {
            // Предаваме статуса към изгледа
            ViewBag.StatusCode = code;

            // Специфични съобщения според кода
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

            // Връщаме изгледа за статус кодове
            return View();
        }
    }
}
