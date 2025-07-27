using AutoShop.Models;
using AutoShop.Services.Interfaces;
using AutoShop.ViewModels.OrderDocument;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AutoShop.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllAsync();
            return View(orders);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            if (!ModelState.IsValid)
            {
                return View(order);
            }

            await _orderService.AddOrderAsync(order);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var order = await _orderService.GetOrderByIdAsync(id.Value);
            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (id != order.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(order);
            }

            try
            {
                await _orderService.UpdateOrderAsync(order);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                // Тук може да добавиш логване на грешката
                throw;
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var order = await _orderService.GetOrderByIdAsync(id.Value);
            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _orderService.DeleteOrderAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // Корекция на метода OrderDocument за избягване на AmbiguousMatchException

        [HttpGet]
        [ActionName("OrderDocument")]
        public IActionResult OrderDocumentGet(int carId)
        {
            var model = new OrderDocumentViewModel
            {
                CarId = carId
            };
            return View(model);
        }

        [HttpPost]
        [ActionName("OrderDocument")]
        [ValidateAntiForgeryToken]
        public IActionResult OrderDocumentPost(OrderDocumentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Тук можеш да обработиш запитването - запис, имейл и т.н.

            TempData["Message"] = "Запитването е изпратено успешно!";
            return RedirectToAction("Index", "Home");
        }
    }
}
