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

        // Конструктор за инжектиране на сервиза за поръчки
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // Действие за показване на всички поръчки
        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllAsync();
            return View(orders);
        }

        // Връща изглед за създаване на нова поръчка
        public IActionResult Create()
        {
            return View();
        }

        // Приема POST заявка за създаване на поръчка
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            // Проверка за валидност на модела
            if (!ModelState.IsValid)
            {
                return View(order);
            }

            // Добавя поръчката чрез сервиза
            await _orderService.AddOrderAsync(order);
            return RedirectToAction(nameof(Index));
        }

        // Връща изглед за редактиране на поръчка по нейния ID
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var order = await _orderService.GetOrderByIdAsync(id.Value);
            if (order == null) return NotFound();

            return View(order);
        }

        // Обработва POST заявка за редакция на поръчка
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            // Проверява дали ID-тата съвпадат, за да избегне грешки
            if (id != order.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(order);
            }

            try
            {
                // Актуализира поръчката чрез сервиза
                await _orderService.UpdateOrderAsync(order);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                // Тук можеш да добавиш логване на грешката
                throw;
            }
        }

        // Връща изглед за потвърждаване на изтриване на поръчка
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var order = await _orderService.GetOrderByIdAsync(id.Value);
            if (order == null) return NotFound();

            return View(order);
        }

        // Обработва POST заявка за изтриване на поръчка
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _orderService.DeleteOrderAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET метод за OrderDocument — за показване на форма с предварително зададен CarId
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

        // POST метод за OrderDocument — за обработка на изпратената форма
        [HttpPost]
        [ActionName("OrderDocument")]
        [ValidateAntiForgeryToken]
        public IActionResult OrderDocumentPost(OrderDocumentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Ако има грешки, връща формата с грешките
                return View(model);
            }

            // Тук можеш да добавиш логика за запазване на заявката, изпращане на имейл и др.

            // Показва съобщение за успешна заявка
            TempData["Message"] = "Запитването е изпратено успешно!";
            return RedirectToAction("Index", "Home");
        }
    }
}
