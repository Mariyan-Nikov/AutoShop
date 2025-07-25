using System.ComponentModel.DataAnnotations;

namespace AutoShop.ViewModels.Settings
{
    public class SettingsViewModel
    {
        [Range(1, 100, ErrorMessage = "Въведи стойност между 1 и 100.")]
        public int ItemsPerPage { get; set; }

        public bool EnableNotifications { get; set; }
    }
}
