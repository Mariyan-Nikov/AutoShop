using AutoShop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutoShop.Services.Interfaces
{
    // Дефинира операциите, които DealerService ще предоставя
    public interface IDealerService
    {
        Task<IEnumerable<Dealer>> GetAllDealersAsync();
        Task<Dealer> GetDealerByIdAsync(int id);
        Task AddDealerAsync(Dealer dealer);
        Task UpdateDealerAsync(Dealer dealer);
        Task DeleteDealerAsync(int id);
        Task<bool> DealerExistsAsync(int id);
    }
}