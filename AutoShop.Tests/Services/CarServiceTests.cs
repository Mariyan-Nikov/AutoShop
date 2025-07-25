using AutoShop.Data;
using AutoShop.Models;
using AutoShop.Services;
using AutoShop.ViewModels.Car;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class CarServiceTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);

        // Seed some cars
        context.Cars.AddRange(
            new Car { Id = 1, Brand = "Toyota", Model = "Corolla", Year = 2020, Price = 15000, RegistrationNumber = "ABC123" },
            new Car { Id = 2, Brand = "Honda", Model = "Civic", Year = 2019, Price = 14000, RegistrationNumber = "XYZ789" },
            new Car { Id = 3, Brand = "Ford", Model = "Focus", Year = 2018, Price = 13000, RegistrationNumber = "FOC456" }
        );
        context.SaveChanges();

        return context;
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCars()
    {
        // Arrange
        var context = GetDbContext();
        var service = new CarService(context);

        // Act
        var cars = await service.GetAllAsync();

        // Assert
        Assert.NotNull(cars);
        Assert.Equal(3, cars.Count());
    }

    [Fact]
    public async Task GetCarByIdAsync_ReturnsCorrectCar()
    {
        var context = GetDbContext();
        var service = new CarService(context);

        var car = await service.GetCarByIdAsync(2);

        Assert.NotNull(car);
        Assert.Equal("Honda", car.Brand);
        Assert.Equal("Civic", car.Model);
    }

    [Fact]
    public async Task AddCarAsync_AddsNewCar()
    {
        var context = GetDbContext();
        var service = new CarService(context);

        var newCar = new Car
        {
            Brand = "BMW",
            Model = "X5",
            Year = 2021,
            Price = 50000,
            RegistrationNumber = "BMW555"
        };

        await service.AddCarAsync(newCar);

        var allCars = await service.GetAllAsync();

        Assert.Contains(allCars, c => c.Brand == "BMW" && c.Model == "X5");
    }

    [Fact]
    public async Task UpdateCarAsync_UpdatesExistingCar()
    {
        var context = GetDbContext();
        var service = new CarService(context);

        var car = await service.GetCarByIdAsync(1);
        car.Price = 16000;

        await service.UpdateCarAsync(car);

        var updatedCar = await service.GetCarByIdAsync(1);

        Assert.Equal(16000, updatedCar.Price);
    }

    [Fact]
    public async Task DeleteCarAsync_RemovesCar()
    {
        var context = GetDbContext();
        var service = new CarService(context);

        await service.DeleteCarAsync(3);

        var deletedCar = await service.GetCarByIdAsync(3);

        Assert.Null(deletedCar);
    }

    [Fact]
    public async Task GetAllAsync_WithSearchAndPaging_ReturnsFilteredCars()
    {
        var context = GetDbContext();
        var service = new CarService(context);

        var result = await service.GetAllAsync("civ", 1, 10);

        Assert.NotNull(result);
        Assert.Single(result.Cars);
        Assert.Equal("Honda", result.Cars.First().Brand);
    }

    [Fact]
    public async Task GetAllAsync_WithPaging_ReturnsPagedResults()
    {
        var context = GetDbContext();
        var service = new CarService(context);

        var result = await service.GetAllAsync(null, 2, 1); // Page 2, 1 car per page

        Assert.NotNull(result);
        Assert.Single(result.Cars);
        Assert.Equal("Honda", result.Cars.First().Brand); // Because ordering is by Id descending
    }
}
