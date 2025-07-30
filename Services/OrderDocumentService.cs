using AutoShop.Data; // DbContext за достъп до базата
using AutoShop.Services.Interfaces; // Контрактът за сервиса
using AutoShop.ViewModels.OrderDocument; // ViewModel за проекция
using Microsoft.EntityFrameworkCore; // EF Core async/LINQ
using System.Collections.Generic; // Колекции
using System.Linq; // LINQ оператори
using System.Threading.Tasks; // Async/await

namespace AutoShop.Services
{
    public class OrderDocumentService : IOrderDocumentService // Сервис за заявки за документи
    {
        private readonly ApplicationDbContext _context; // DI на контекста

        public OrderDocumentService(ApplicationDbContext context) // Конструктор с инжекция
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderDocumentViewModel>> GetAllRequestsAsync() // Чете активни заявки като ViewModel
        {
            return await _context.OrderDocuments
                .AsNoTracking() // Без тракинг за по-бързо четене
                .Where(o => o.IsActive) // Само активните записи
                .Select(o => new OrderDocumentViewModel // Проекция към лек модел за UI
                {
                    CarId = o.CarId, // Идентификатор на колата
                    FullName = o.FullName, // Име на клиента
                    PhoneNumber = o.PhoneNumber, // Телефон
                    Email = o.Email, // Имейл
                    Message = o.Message, // Съобщение
                    PreferredDate = o.PreferredDate // Предпочитана дата
                    // Добави CreatedOn във ViewModel, ако ти трябва
                })
                .ToListAsync(); // Изпълнение на заявката асинхронно
        }
    }
}
