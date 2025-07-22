using AutoShop.Models;
using AutoShop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using AutoShop.Services.Interfaces;


namespace AutoShop.Controllers
{
    public class OrderItemController : Controller
    {
        private readonly IOrderItemService _orderItemService;
        private readonly ICarService _carService;
        private readonly IOrderService _orderService;

        public OrderItemController(
            IOrderItemService orderItemService,
            ICarService carService,
            IOrderService orderService)
        {
            _orderItemService = orderItemService;
            _carService = carService;
            _orderService = orderService;
        }

        // GET: /OrderItem
        public async Task<IActionResult> Index()
        {
            var items = await _orderItemService.GetAllAsync();
            return View(items);
        }

        // GET: /OrderItem/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _orderItemService.GetByIdAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        // GET: /OrderItem/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Cars = new SelectList(await _carService.GetAllAsync(), "Id", "Model");
            ViewBag.Orders = new SelectList(await _orderService.GetAllAsync(), "Id", "Id");

            return View();
        }

        // POST: /OrderItem/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderItem item)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Cars = new SelectList(await _carService.GetAllAsync(), "Id", "Model");
                ViewBag.Orders = new SelectList(await _orderService.GetAllAsync(), "Id", "Id");
                return View(item);
            }

            await _orderItemService.AddAsync(item);
            return RedirectToAction(nameof(Index));
        }

        // GET: /OrderItem/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _orderItemService.GetByIdAsync(id);
            if (item == null) return NotFound();

            ViewBag.Cars = new SelectList(await _carService.GetAllAsync(), "Id", "Model", item.CarId);
            ViewBag.Orders = new SelectList(await _orderService.GetAllAsync(), "Id", "Id", item.OrderId);

            return View(item);
        }

        // POST: /OrderItem/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderItem item)
        {
            if (id != item.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.Cars = new SelectList(await _carService.GetAllAsync(), "Id", "Model", item.CarId);
                ViewBag.Orders = new SelectList(await _orderService.GetAllAsync(), "Id", "Id", item.OrderId);
                return View(item);
            }

            await _orderItemService.UpdateAsync(item);
            return RedirectToAction(nameof(Index));
        }

        // GET: /OrderItem/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _orderItemService.GetByIdAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        // POST: /OrderItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _orderItemService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
