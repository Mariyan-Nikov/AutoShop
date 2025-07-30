using System.Diagnostics;
using AutoShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace AutoShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // ����������� �� ����������� �� �����, ����� ���� �� �� �������� �� ��������� �� ������� � ������
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // �������� �� ��������� ��������
        public IActionResult Index()
        {
            return View();
        }

        // �������� �� ���������� � �������� �� �������������
        public IActionResult Privacy()
        {
            return View();
        }

        // �������� �� ��������� �� ������
        // ���������� ������� ���������� �� �������� - � ������ ���� ��������
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // ������� ���������� �� �������� �� ��������� �� HTTP ��������
            var feature = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

            if (feature != null)
            {
                // ��� ��� ������, �������� ����������� � ���� ��� ������� ���� ViewBag
                ViewBag.ErrorMessage = feature.Error.Message;
                ViewBag.Path = feature.Path;
            }
            else
            {
                // ��� ���� ��������� ������, �������� ���� ���������
                ViewBag.ErrorMessage = "���������� ������";
                ViewBag.Path = "";
            }

            // ������� ������� �� ������
            return View();
        }

        // �������� �� ������ ������ (�������� 404, 500 � ��.)
        public IActionResult StatusCode(int code)
        {
            // ��������� ������� ��� �������
            ViewBag.StatusCode = code;

            // ���������� ��������� ������ ����
            switch (code)
            {
                case 404:
                    ViewBag.Message = "���������� �� � ��������.";
                    break;
                case 500:
                    ViewBag.Message = "�������� ������ �� �������.";
                    break;
                default:
                    ViewBag.Message = "�������� ������.";
                    break;
            }

            // ������� ������� �� ������ ������
            return View();
        }
    }
}
