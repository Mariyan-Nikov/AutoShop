using System;
using System.Collections.Generic;

namespace AutoShop.ViewModels.Car
{
    public class CarQueryModel
    {
        public int CarsPerPage { get; set; } = 5;           // размер на страница
        public string? SearchTerm { get; set; }             // търсене
        public int CurrentPage { get; set; } = 1;           // текуща страница
        public int TotalCars { get; set; }                  // общ брой
        public IEnumerable<CarViewModel> Cars { get; set; } = new List<CarViewModel>(); // резултати
        public int TotalPages => (int)Math.Ceiling((double)TotalCars / CarsPerPage);    // страници
    }
}
