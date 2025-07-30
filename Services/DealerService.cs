using AutoShop.Data;
using AutoShop.Models;
using AutoShop.Services.Interfaces; // Интерфейсът е нужен тук
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutoShop.Services
{
    // Сервис за дилъри (работи с EF Core контекста)
    public class DealerService : IDealerService
    {
        private readonly ApplicationDbContext _context; // DI за DbContext

        public DealerService(ApplicationDbContext context) // Инжектиране на контекста
        {
            _context = context;
        }

        public async Task<IEnumerable<Dealer>> GetAllDealersAsync() // Всички дилъри (по избор: AsNoTracking)
        {
            return await _context.Dealers.ToListAsync();
        }

        public async Task<Dealer> GetDealerByIdAsync(int id) // Заб.: FindAsync може да върне null → обмисли Dealer?
        {
            return await _context.Dealers.FindAsync(id);
        }

        public async Task AddDealerAsync(Dealer dealer) // Добавяне и запис
        {
            _context.Dealers.Add(dealer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDealerAsync(Dealer dealer) // Обновяване (по-ясно: _context.Dealers.Update)
        {
            _context.Update(dealer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDealerAsync(int id) // Изтриване с null проверка
        {
            var dealer = await _context.Dealers.FindAsync(id);
            if (dealer != null)
            {
                _context.Dealers.Remove(dealer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DealerExistsAsync(int id) // Проверка за съществуване по Id
        {
            return await _context.Dealers.AnyAsync(e => e.Id == id);
        }
    }
}
