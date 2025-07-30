using System.ComponentModel.DataAnnotations; // За Data Annotations - валидации и атрибути

namespace AutoShop.Models
{
    public class Car
    {
        public int Id { get; set; }
        // Уникален идентификатор на колата в базата данни

        [Required(ErrorMessage = "Brand is required")]
        [StringLength(100, ErrorMessage = "Brand cannot be longer than 100 characters")]
        public string Brand { get; set; } = null!;
        // Марка на колата (задължително поле, максимум 100 символа)

        [Required(ErrorMessage = "Model is required")]
        [StringLength(100, ErrorMessage = "Model cannot be longer than 100 characters")]
        public string Model { get; set; } = null!;
        // Модел на колата (задължително поле, максимум 100 символа)

        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
        public int Year { get; set; }
        // Година на производство - стойност между 1900 и 2100

        [Range(0.01, 1000000, ErrorMessage = "Price must be greater than zero")]
        public decimal Price { get; set; }
        // Цена на колата - задължително положително число

        public string? Description { get; set; }
        // Описание на колата - по избор, може да е null

        [Required(ErrorMessage = "RegistrationNumber is required")]
        [StringLength(50, ErrorMessage = "RegistrationNumber cannot be longer than 50 characters")]
        public string RegistrationNumber { get; set; } = null!;
        // Регистрационен номер на колата (задължително поле, максимум 50 символа)

        public string? ImageFileName { get; set; }
        // Име на файл с изображение (по избор, може да е null)
    }
}
