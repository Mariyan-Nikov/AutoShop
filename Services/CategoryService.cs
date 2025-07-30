using AutoShop.Data;
using AutoShop.Models;
using AutoShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutoShop.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context; // DI за достъп до базата

        public CategoryService(ApplicationDbContext context) // Инжектиране на контекста
        {
            _context = context;
        }

        // Взема всички категории от базата
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        // Връща категория по ID
        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        // Добавя нова категория
        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        // Обновява категория
        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        // Изтрива категория по ID, ако съществува
        public async Task DeleteAsync(int id)
        {
            var category = await GetByIdAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
