using AutoShop.Models;               // За моделите Review и Car
using System.Collections.Generic;   // За IEnumerable<>
using System.Threading.Tasks;       // За асинхронни операции

// Интерфейс за услугата, която управлява ревюта
public interface IReviewService
{
    // Връща всички ревюта асинхронно
    Task<IEnumerable<Review>> GetAllReviewsAsync();

    // Връща конкретно ревю по ID асинхронно
    Task<Review?> GetReviewByIdAsync(int id);

    // Добавя ново ревю асинхронно
    Task AddReviewAsync(Review review);

    // Обновява съществуващо ревю асинхронно
    Task UpdateReviewAsync(Review review);

    // Изтрива ревю по ID асинхронно
    Task DeleteReviewAsync(int id);

    // Връща списък с всички коли (за dropdown в UI)
    IEnumerable<Car> GetAllCars();
}
