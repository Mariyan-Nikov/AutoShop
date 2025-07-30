using AutoShop.Models;               // За модела Order
using System.Collections.Generic;   // За IEnumerable<>
using System.Threading.Tasks;       // За асинхронни операции

namespace AutoShop.Services.Interfaces
{
    // Интерфейс за услугата, която управлява поръчки
    public interface IOrderService
    {
        // Връща всички поръчки
        Task<IEnumerable<Order>> GetAllAsync();

        // Връща поръчка по ID, ако съществува
        Task<Order?> GetOrderByIdAsync(int id);

        // Добавя нова поръчка
        Task AddOrderAsync(Order order);

        // Обновява съществуваща поръчка
        Task UpdateOrderAsync(Order order);

        // Изтрива поръчка по ID
        Task DeleteOrderAsync(int id);
    }
}
