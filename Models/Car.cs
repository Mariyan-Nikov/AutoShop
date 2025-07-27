using System.ComponentModel.DataAnnotations;

namespace AutoShop.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        [StringLength(100, ErrorMessage = "Brand cannot be longer than 100 characters")]
        public string Brand { get; set; } = null!;

        [Required(ErrorMessage = "Model is required")]
        [StringLength(100, ErrorMessage = "Model cannot be longer than 100 characters")]
        public string Model { get; set; } = null!;

        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
        public int Year { get; set; }

        [Range(0.01, 1000000, ErrorMessage = "Price must be greater than zero")]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "RegistrationNumber is required")]
        [StringLength(50, ErrorMessage = "RegistrationNumber cannot be longer than 50 characters")]
        public string RegistrationNumber { get; set; } = null!;

        public string? ImageFileName { get; set; }
    }
}
