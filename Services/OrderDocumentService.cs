using AutoShop.Data; // Тук е твоят DbContext
using AutoShop.Services.Interfaces;
using AutoShop.ViewModels.OrderDocument;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoShop.Services
{
    public class OrderDocumentService : IOrderDocumentService
    {
        private readonly ApplicationDbContext _context;

        public OrderDocumentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderDocumentViewModel>> GetAllRequestsAsync()
        {
            return await _context.OrderDocuments
                .Where(o => o.IsActive)
                .Select(o => new OrderDocumentViewModel
                {
                    CarId = o.CarId,
                    FullName = o.FullName,
                    PhoneNumber = o.PhoneNumber,
                    Email = o.Email,
                    Message = o.Message,
                    PreferredDate = o.PreferredDate
                    // Ако искаш да върнеш CreatedOn, добави го във ViewModel
                })
                .ToListAsync();
        }
    }
}
