using AutoShop.Models;               // Импортира моделите, включително Dealer
using System.Collections.Generic;    // За IEnumerable<>
using System.Threading.Tasks;        // За асинхронни методи

namespace AutoShop.Services.Interfaces
{
    // Интерфейс за операциите, които DealerService трябва да имплементира
    public interface IDealerService
    {
        // Връща всички дилъри
        Task<IEnumerable<Dealer>> GetAllDealersAsync();

        // Връща дилър по ID
        Task<Dealer> GetDealerByIdAsync(int id);

        // Добавя нов дилър
        Task AddDealerAsync(Dealer dealer);

        // Обновява съществуващ дилър
        Task UpdateDealerAsync(Dealer dealer);

        // Изтрива дилър по ID
        Task DeleteDealerAsync(int id);

        // Проверява дали дилър с даден ID съществува
        Task<bool> DealerExistsAsync(int id);
    }
}
