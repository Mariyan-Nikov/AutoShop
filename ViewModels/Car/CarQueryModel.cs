using AutoShop.ViewModels.Car;
using System;
using System.Collections.Generic;
namespace AutoShop.ViewModels.Car;

public class CarQueryModel
{
    public int CarsPerPage { get; set; } = 5;
    public string? SearchTerm { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int TotalCars { get; set; }
    public IEnumerable<CarViewModel> Cars { get; set; } = new List<CarViewModel>();

    public int TotalPages => (int)Math.Ceiling((double)TotalCars / CarsPerPage);
}
