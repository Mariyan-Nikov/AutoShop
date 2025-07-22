using AutoShop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IReviewService
{
    Task<IEnumerable<Review>> GetAllReviewsAsync();
    Task<Review?> GetReviewByIdAsync(int id);
    Task AddReviewAsync(Review review);
    Task UpdateReviewAsync(Review review);
    Task DeleteReviewAsync(int id);

    // Добави този метод за dropdown-а с коли:
    IEnumerable<Car> GetAllCars();
}
