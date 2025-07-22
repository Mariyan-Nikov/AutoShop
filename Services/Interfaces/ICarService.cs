using AutoShop.ViewModels.Car;
using AutoShop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutoShop.Services.Interfaces
{
    public interface ICarService
    {
        Task<IEnumerable<Car>> GetAllAsync();

        Task<Car?> GetCarByIdAsync(int id);

        Task AddCarAsync(Car car);

        Task UpdateCarAsync(Car car);

        Task DeleteCarAsync(int id);

        Task<CarQueryModel> GetAllAsync(string? searchTerm, int currentPage, int carsPerPage);
    }
}
