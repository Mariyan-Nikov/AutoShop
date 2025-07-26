using AutoShop.Models;
using AutoShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

namespace AutoShop.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;

        public OrderController(IOrderService orderService, ICustomerService customerService)
        {
            _orderService = orderService;
            _customerService = customerService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllAsync();
            return View(orders);
        }

        public async Task<IActionResult> Create()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            ViewBag.Customers = new SelectList(customers, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            if (!ModelState.IsValid)
            {
                var customers = await _customerService.GetAllCustomersAsync();
                ViewBag.Customers = new SelectList(customers, "Id", "Name", order.CustomerId);
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

            var customers = await _customerService.GetAllCustomersAsync();
            ViewBag.Customers = new SelectList(customers, "Id", "Name", order.CustomerId);

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (id != order.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                var customers = await _customerService.GetAllCustomersAsync();
                ViewBag.Customers = new SelectList(customers, "Id", "Name", order.CustomerId);
                return View(order);
            }

            try
            {
                await _orderService.UpdateOrderAsync(order);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                // Log the error if needed
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
    }
}
