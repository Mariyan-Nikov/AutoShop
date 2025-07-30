using AutoShop.Models;
using AutoShop.Services.Interfaces;
using AutoShop.ViewModels.Car;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace AutoShop.Controllers
{
    public class CarController : Controller
    {
        private readonly ICarService _carService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Конструктор с dependency injection за услугата и информация за хост средата
        public CarController(ICarService carService, IWebHostEnvironment webHostEnvironment)
        {
            _carService = carService;
            _webHostEnvironment = webHostEnvironment;
        }

        // Действие Index: показва списък с коли с опция за търсене и странициране
        public async Task<IActionResult> Index(string? searchTerm, int currentPage = 1)
        {
            int carsPerPage = 5; // Брой коли на страница
            var model = await _carService.GetAllAsync(searchTerm, currentPage, carsPerPage);
            return View(model);
        }

        // Действие Details: показва детайлите за конкретна кола по нейното ID
        public async Task<IActionResult> Details(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null)
                return NotFound(); // Връща 404 ако колата не съществува

            return View(car);
        }

        // GET: Създаване на нова кола - връща празна форма
        public IActionResult Create()
        {
            return View();
        }

        // POST: Създаване на нова кола с качване на изображение
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Car car, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
                return View(car);

            // Качване на изображението, ако има такова
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

                car.ImageFileName = fileName; // Записва името на файла в модела
            }

            await _carService.AddCarAsync(car);
            return RedirectToAction(nameof(Index));
        }

        // GET: Редактиране на кола по ID
        public async Task<IActionResult> Edit(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null)
                return NotFound();

            return View(car);
        }

        // POST: Редактиране на кола с опция за смяна на изображение
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Car car, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
                return View(car);

            // Ако има качване на ново изображение, го записва
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

            await _carService.UpdateCarAsync(car);
            return RedirectToAction(nameof(Index));
        }

        // GET: Потвърждение за изтриване на кола
        public async Task<IActionResult> Delete(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null)
                return NotFound();

            return View(car);
        }

        // POST: Изтриване на кола
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _carService.DeleteCarAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
