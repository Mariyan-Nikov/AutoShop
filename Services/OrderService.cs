using AutoShop.Data;
using AutoShop.Models;
using AutoShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutoShop.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context; // DI за EF Core контекста

        public OrderService(ApplicationDbContext context) // Инжектиране на контекста
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllAsync() // Взема всички поръчки (по избор: AsNoTracking)
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id) // Връща поръчка по Id (възможно null)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task AddOrderAsync(Order order) // Добавя нова поръчка и записва
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrderAsync(Order order) // Обновява поръчка (Update → Modified)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(int id) // Изтрива поръчка по Id, ако съществува
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }
    }
}
