using AutoShop.Models;
using AutoShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;


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
        ViewData["Customers"] = new SelectList(customers, "Id", "FullName");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Order order)
    {
        if (ModelState.IsValid)
        {
            await _orderService.AddOrderAsync(order);
            return RedirectToAction(nameof(Index));
        }

        var customers = await _customerService.GetAllCustomersAsync();
        ViewData["Customers"] = new SelectList(customers, "Id", "FullName", order.CustomerId);
        return View(order);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _orderService.GetOrderByIdAsync(id.Value);
        if (order == null)
        {
            return NotFound();
        }

        var customers = await _customerService.GetAllCustomersAsync();
        ViewData["Customers"] = new SelectList(customers, "Id", "FullName", order.CustomerId);

        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Order order)
    {
        if (id != order.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _orderService.UpdateOrderAsync(order);
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        var customers = await _customerService.GetAllCustomersAsync();
        ViewData["Customers"] = new SelectList(customers, "Id", "FullName", order.CustomerId);
        return View(order);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _orderService.GetOrderByIdAsync(id.Value);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    [HttpPost, ActionName("DeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _orderService.DeleteOrderAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
