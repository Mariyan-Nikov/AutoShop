using AutoShop.Models;               // За модела Category
using System.Collections.Generic;    // За IEnumerable<>
using System.Threading.Tasks;        // За асинхронни операции

namespace AutoShop.Services.Interfaces
{
    public interface ICategoryService
    {
        // Връща всички категории
        Task<IEnumerable<Category>> GetAllAsync();

        // Връща категория по ID или null, ако няма такава
        Task<Category?> GetByIdAsync(int id);

        // Добавя нова категория
        Task AddAsync(Category category);

        // Обновява съществуваща категория
        Task UpdateAsync(Category category);

        // Изтрива категория по ID
        Task DeleteAsync(int id);
    }
}
