using AutoShop.ViewModels.OrderDocument;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutoShop.Services.Interfaces
{
    public interface IOrderDocumentService
    {
        Task<IEnumerable<OrderDocumentViewModel>> GetAllRequestsAsync();
    }
}
