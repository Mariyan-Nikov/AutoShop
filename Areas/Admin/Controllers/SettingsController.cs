using AutoShop.Data;
using AutoShop.Data.Entities;
using AutoShop.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Settings
        public async Task<IActionResult> Index()
        {
            var settings = await _context.Settings.ToListAsync();

            var model = new SettingsViewModel
            {
                ItemsPerPage = GetIntSetting(settings, "ItemsPerPage", 10),
                EnableNotifications = GetBoolSetting(settings, "EnableNotifications", true)
            };

            return View(model);
        }

        // POST: /Admin/Settings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await SetSetting("ItemsPerPage", model.ItemsPerPage.ToString());
            await SetSetting("EnableNotifications", model.EnableNotifications.ToString());

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Настройките са запазени успешно.";
            return RedirectToAction(nameof(Index));
        }

        private int GetIntSetting(List<Setting> settings, string key, int defaultValue)
        {
            var setting = settings.FirstOrDefault(s => s.Key == key);
            if (setting != null && int.TryParse(setting.Value, out int val))
            {
                return val;
            }
            return defaultValue;
        }

        private bool GetBoolSetting(List<Setting> settings, string key, bool defaultValue)
        {
            var setting = settings.FirstOrDefault(s => s.Key == key);
            if (setting != null && bool.TryParse(setting.Value, out bool val))
            {
                return val;
            }
            return defaultValue;
        }

        private async Task SetSetting(string key, string value)
        {
            var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Key == key);
            if (setting == null)
            {
                setting = new Setting { Key = key, Value = value };
                await _context.Settings.AddAsync(setting);
            }
            else
            {
                setting.Value = value;
                _context.Settings.Update(setting);
            }
        }
    }
}
