using AutoShop.Models;
using AutoShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AutoShop.Controllers
{
    public class CarTestController : Controller
    {
        private readonly ICarService _carService;

        // Конструкторът трябва да е с името на класа!
        public CarTestController(ICarService carService)
        {
            _carService = carService;
        }

        // GET: /CarTest/Index
        public async Task<IActionResult> Index(string? searchTerm, int page = 1)
        {
            const int CarsPerPage = 10;

            var model = await _carService.GetAllAsync(searchTerm, page, CarsPerPage);

            return View(model);
        }

        // GET: /CarTest/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // GET: /CarTest/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /CarTest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Car car)
        {
            if (!ModelState.IsValid)
            {
                return View(car);
            }

            await _carService.AddCarAsync(car);
            return RedirectToAction(nameof(Index));
        }

        // GET: /CarTest/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: /CarTest/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Car car)
        {
            if (id != car.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(car);
            }

            await _carService.UpdateCarAsync(car);
            return RedirectToAction(nameof(Index));
        }

        // GET: /CarTest/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: /CarTest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _carService.DeleteCarAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
