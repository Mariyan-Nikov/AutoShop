using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AutoShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SettingsController : Controller
    {
        // GET: /Admin/Settings
        public IActionResult Index()
        {
            // Примерни настройки, можеш да ги замениш с реални от база или config
            var model = new SettingsViewModel
            {
                SiteName = "AutoShop",
                ItemsPerPage = 10,
                EnableNotifications = true
            };
            return View(model);
        }

        // POST: /Admin/Settings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(SettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Тук можеш да запишеш настройките (например в база или файл)
            // За сега просто връщаме обратно на страницата

            TempData["SuccessMessage"] = "Настройките са запазени успешно.";
            return View(model);
        }
    }

    public class SettingsViewModel
    {
        [Required]
        public string SiteName { get; set; } = null!;

        [Range(1, 100)]
        public int ItemsPerPage { get; set; }

        public bool EnableNotifications { get; set; }
    }
}
