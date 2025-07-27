using AutoShop.Services.Interfaces;
using AutoShop.ViewModels.OrderDocument;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AutoShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderDocumentController : Controller
    {
        private readonly IOrderDocumentService _orderDocumentService;

        public OrderDocumentController(IOrderDocumentService orderDocumentService)
        {
            _orderDocumentService = orderDocumentService;
        }

        public async Task<IActionResult> All()
        {
            var allRequests = await _orderDocumentService.GetAllRequestsAsync();
            return View(allRequests);
        }
    }
}
