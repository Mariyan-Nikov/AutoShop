namespace AutoShop.ViewModels.Car
{
    public class CarViewModel
    {
        public int Id { get; set; }
        public string Brand { get; set; } = null!;

        public string Model { get; set; } = null!;

        public int Year { get; set; }

        public string RegistrationNumber { get; set; } = null!;

        public string? ImageFileName { get; set; }
    }
}
