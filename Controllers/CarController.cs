using AutoShop.Hubs;
using AutoShop.Models;
using AutoShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AutoShop.Controllers
{
    [Authorize]
    public class CarController : Controller
    {
        private readonly ICarService _carService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHubContext<CarHub> _hubContext;

        public CarController(
            ICarService carService,
            IWebHostEnvironment webHostEnvironment,
            IHubContext<CarHub> hubContext)
        {
            _carService = carService;
            _webHostEnvironment = webHostEnvironment;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index(string searchTerm = "", int page = 1)
        {
            int carsPerPage = 10; // може да е и от конфигурация
            var cars = await _carService.GetAllAsync(searchTerm, page, carsPerPage);
            return View(cars);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Car car, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
                return View(car);

            if (imageFile != null && imageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                car.ImageFileName = fileName;
            }

            await _carService.AddCarAsync(car);

            // Изпращане на известие чрез SignalR до всички клиенти
            await _hubContext.Clients.All.SendAsync("ReceiveCarNotification", $"{car.Brand} {car.Model}");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null) return NotFound();
            return View(car);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null) return NotFound();
            return View(car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Car car)
        {
            if (!ModelState.IsValid)
                return View(car);

            await _carService.UpdateCarAsync(car);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null) return NotFound();
            return View(car);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _carService.DeleteCarAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
