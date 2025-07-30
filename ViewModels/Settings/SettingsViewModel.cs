using System.ComponentModel.DataAnnotations;

namespace AutoShop.ViewModels.Settings
{
    public class SettingsViewModel
    {
        // Брой елементи на страница (валидира се от 1 до 100)
        [Range(1, 100, ErrorMessage = "Въведи стойност между 1 и 100.")]
        public int ItemsPerPage { get; set; }

        // Включване или изключване на нотификациите
        public bool EnableNotifications { get; set; }
    }
}
