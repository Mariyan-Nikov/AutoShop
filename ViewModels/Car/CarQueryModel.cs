using AutoShop.ViewModels.Car;

public class CarQueryModel
{
    public int CarsPerPage { get; set; } = 5;  // това е инстанционно свойство, а не static
    public string? SearchTerm { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int TotalCars { get; set; }
    public IEnumerable<CarViewModel> Cars { get; set; } = new List<CarViewModel>();
}
