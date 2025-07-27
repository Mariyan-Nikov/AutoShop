using AutoShop.ViewModels.Car;
using AutoShop.Models;
using System.Threading.Tasks;

namespace AutoShop.Services.Interfaces
{
    public interface ICarService
    {
        Task<Car?> GetCarByIdAsync(int id);

        Task AddCarAsync(Car car);

        Task UpdateCarAsync(Car car);

        Task DeleteCarAsync(int id);

        Task<CarQueryModel> GetAllAsync(string? searchTerm, int currentPage, int carsPerPage);

        // ✅ Нов overload за извикване без параметри
        Task<IEnumerable<CarViewModel>> GetAllAsync();


    }
}
