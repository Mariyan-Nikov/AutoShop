using AutoShop.ViewModels.Car; // За моделите, използвани за пренос на данни към View слоя
using AutoShop.Models;         // За основния Car модел от домейна
using System.Threading.Tasks;  // За асинхронни операции

namespace AutoShop.Services.Interfaces
{
    public interface ICarService
    {
        // Връща кола по дадено ID или null, ако няма такава
        Task<Car?> GetCarByIdAsync(int id);

        // Добавя нова кола в базата данни
        Task AddCarAsync(Car car);

        // Обновява съществуваща кола
        Task UpdateCarAsync(Car car);

        // Изтрива кола по ID
        Task DeleteCarAsync(int id);

        // Връща резултат с търсене и странициране (carsPerPage на страница)
        Task<CarQueryModel> GetAllAsync(string? searchTerm, int currentPage, int carsPerPage);

        // ✅ Нов метод за връщане на всички коли без параметри (например за dropdown)
        Task<IEnumerable<CarViewModel>> GetAllAsync();
    }
}
