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
        private readonly ApplicationDbContext _context;

        public CarService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Връща всички коли (без филтри, за вътрешна употреба)
        public async Task<IEnumerable<Car>> GetAllAsync()
        {
            return await _context.Cars.ToListAsync();
        }

        // Връща кола по ID
        public async Task<Car?> GetCarByIdAsync(int id)
        {
            return await _context.Cars.FindAsync(id);
        }

        // Добавяне на нова кола
        public async Task AddCarAsync(Car car)
        {
            await _context.Cars.AddAsync(car);
            await _context.SaveChangesAsync();
        }

        // Обновяване на съществуваща кола
        public async Task UpdateCarAsync(Car car)
        {
            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
        }

        // Изтриване на кола по ID
        public async Task DeleteCarAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }
        }

        // Връща коли с търсене и странициране
        public async Task<CarQueryModel> GetAllAsync(string? searchTerm, int currentPage, int carsPerPage)
        {
            var carsQuery = _context.Cars.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                carsQuery = carsQuery.Where(c =>
                    c.Brand.ToLower().Contains(searchTerm) ||
                    c.Model.ToLower().Contains(searchTerm) ||
                    c.RegistrationNumber.ToLower().Contains(searchTerm));
            }

            var totalCars = await carsQuery.CountAsync();

            var cars = await carsQuery
                .OrderByDescending(c => c.Id)
                .Skip((currentPage - 1) * carsPerPage)
                .Take(carsPerPage)
                .Select(c => new CarViewModel
                {
                    Id = c.Id,
                    Brand = c.Brand,
                    Model = c.Model,
                    Year = c.Year,
                    RegistrationNumber = c.RegistrationNumber
                })
                .ToListAsync();

            return new CarQueryModel
            {
                Cars = cars,
                SearchTerm = searchTerm,
                CurrentPage = currentPage,
                TotalCars = totalCars,
                CarsPerPage = carsPerPage
            };
        }

        public async Task CreateAsync(Car newCar)
        {
            throw new NotImplementedException();
        }

        public async Task GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(int id, Car updatedCar)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
