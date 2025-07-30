using AutoShop.ViewModels.OrderDocument; // За OrderDocumentViewModel
using System.Collections.Generic;         // За IEnumerable<>
using System.Threading.Tasks;             // За асинхронни методи

namespace AutoShop.Services.Interfaces
{
    // Интерфейс за услугата, управляваща заявки за документи за поръчки
    public interface IOrderDocumentService
    {
        // Връща всички активни заявки за документи (OrderDocumentViewModel)
        Task<IEnumerable<OrderDocumentViewModel>> GetAllRequestsAsync();
    }
}
