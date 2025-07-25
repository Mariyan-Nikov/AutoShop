using AutoShop.Data;
using AutoShop.Models;
using AutoShop.Services.Interfaces; // Уверете се, че този using е наличен
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutoShop.Services
{
    // Имплементация на IDealerService, която работи директно с DbContext
    public class DealerService : IDealerService
    {
        private readonly ApplicationDbContext _context;

        public DealerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Dealer>> GetAllDealersAsync()
        {
            return await _context.Dealers.ToListAsync();
        }

        public async Task<Dealer> GetDealerByIdAsync(int id)
        {
            return await _context.Dealers.FindAsync(id);
        }

        public async Task AddDealerAsync(Dealer dealer)
        {
            _context.Dealers.Add(dealer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDealerAsync(Dealer dealer)
        {
            _context.Update(dealer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDealerAsync(int id)
        {
            var dealer = await _context.Dealers.FindAsync(id);
            if (dealer != null)
            {
                _context.Dealers.Remove(dealer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DealerExistsAsync(int id)
        {
            return await _context.Dealers.AnyAsync(e => e.Id == id);
        }
    }
}