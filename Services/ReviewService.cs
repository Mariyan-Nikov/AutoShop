using AutoShop.Data;
using AutoShop.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _context; // EF Core контекст за достъп до базата

    public ReviewService(ApplicationDbContext context) // Конструктор с DI на контекста
    {
        _context = context;
    }

    public async Task<IEnumerable<Review>> GetAllReviewsAsync() // Всички ревюта с колите им (Include Car)
    {
        return await _context.Reviews.Include(r => r.Car).ToListAsync();
    }

    public async Task<Review?> GetReviewByIdAsync(int id) // Ревю по Id (с Include Car)
    {
        return await _context.Reviews.Include(r => r.Car).FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task AddReviewAsync(Review review) // Добавяне на ново ревю
    {
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateReviewAsync(Review review) // Обновяване на ревю
    {
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteReviewAsync(int id) // Изтриване по Id (null проверка)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review != null)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }

    public IEnumerable<Car> GetAllCars() // Връща колите за dropdown, сортирани по марка и модел
    {
        return _context.Cars.OrderBy(c => c.Brand).ThenBy(c => c.Model).ToList();
    }
}
