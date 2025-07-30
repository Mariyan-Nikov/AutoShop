using AutoShop.Data;
using AutoShop.Models;
using AutoShop.Services.Interfaces;
using AutoShop.ViewModels.Car;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoShop.Services
{
    public class CarService : ICarService
    {
        private readonly ApplicationDbContext _context; // DI за EF Core контекста

        public CarService(ApplicationDbContext context) // Конструктор за инжектиране на контекста
        {
            _context = context;
        }

        // Списък от коли за dropdown/вътрешни нужди (може AsNoTracking за четене)
        public async Task<IEnumerable<CarViewModel>> GetAllAsync()
        {
            return await _context.Cars
                .Select(c => new CarViewModel
                {
                    Id = c.Id,
                    Brand = c.Brand,
                    Model = c.Model,
                    Year = c.Year,
                    RegistrationNumber = c.RegistrationNumber
                })
                .ToListAsync();
        }

        // Връща кола по ID (FindAsync ползва кеш + ключ)
        public async Task<Car?> GetCarByIdAsync(int id)
        {
            return await _context.Cars.FindAsync(id);
        }

        // Добавяне на нова кола (SaveChangesAsync комитва)
        public async Task AddCarAsync(Car car)
        {
            await _context.Cars.AddAsync(car);
            await _context.SaveChangesAsync();
        }

        // Обновяване на съществуваща кола (Update маркира като Modified)
        public async Task UpdateCarAsync(Car car)
        {
            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
        }

        // Изтриване на кола по ID (проверка за null преди Remove)
        public async Task DeleteCarAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }
        }

        // Търсене и странициране на коли (search + paging + projection)
        public async Task<CarQueryModel> GetAllAsync(string? searchTerm, int currentPage, int carsPerPage)
        {
            var carsQuery = _context.Cars.AsQueryable(); // База за филтриране (може AsNoTracking)

            if (!string.IsNullOrWhiteSpace(searchTerm)) // Нормализиране на заявката
            {
                searchTerm = searchTerm.ToLower();
                carsQuery = carsQuery.Where(c =>
                    c.Brand.ToLower().Contains(searchTerm) ||
                    c.Model.ToLower().Contains(searchTerm) ||
                    c.RegistrationNumber.ToLower().Contains(searchTerm));
            }

            var totalCars = await carsQuery.CountAsync(); // Общо за пагинацията

            var cars = await carsQuery
                .OrderByDescending(c => c.Id) // Подредба по ново към старо
                .Skip((currentPage - 1) * carsPerPage) // Прескачане за страницата
                .Take(carsPerPage) // Лимит на страница
                .Select(c => new CarViewModel
                {
                    Id = c.Id,
                    Brand = c.Brand,
                    Model = c.Model,
                    Year = c.Year,
                    RegistrationNumber = c.RegistrationNumber
                })
                .ToListAsync(); // Материализация на резултатите

            return new CarQueryModel
            {
                Cars = cars, // Текуща страница резултати
                SearchTerm = searchTerm, // Ехо на търсенето
                CurrentPage = currentPage, // Номер на страница
                TotalCars = totalCars, // Брой за всички страници
                CarsPerPage = carsPerPage // Размер на страница
            };
        }
    }
}
