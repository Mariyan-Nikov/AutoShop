using AutoShop.Data;
using AutoShop.Models;
using AutoShop.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class OrderItemServiceTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);

        // Seed Orders
        context.Orders.AddRange(
            new Order
            {
                Id = 1,
                CustomerId = 1,
                OrderDate = DateTime.UtcNow.AddDays(-5),
                Status = "Pending"
            },
            new Order
            {
                Id = 2,
                CustomerId = 2,
                OrderDate = DateTime.UtcNow.AddDays(-2),
                Status = "Completed"
            }
        );

        // Seed Cars
        context.Cars.AddRange(
            new Car { Id = 1, Brand = "Toyota", Model = "Corolla", Year = 2020, Price = 15000, RegistrationNumber = "ABC123" },
            new Car { Id = 2, Brand = "Honda", Model = "Civic", Year = 2019, Price = 14000, RegistrationNumber = "XYZ789" }
        );

        // Seed OrderItems
        context.OrderItems.AddRange(
            new OrderItem { Id = 1, OrderId = 1, CarId = 1, Quantity = 2 },
            new OrderItem { Id = 2, OrderId = 2, CarId = 2, Quantity = 1 }
        );

        context.SaveChanges();

        return context;
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllOrderItems()
    {
        // Arrange
        var context = GetDbContext();
        var service = new OrderItemService(context);

        // Act
        var items = await service.GetAllAsync();

        // Assert
        Assert.NotNull(items);
        Assert.Equal(2, ((List<OrderItem>)items).Count);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectOrderItem()
    {
        // Arrange
        var context = GetDbContext();
        var service = new OrderItemService(context);

        // Act
        var item = await service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(item);
        Assert.Equal(1, item.Id);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(1, item.OrderId);
        Assert.Equal(1, item.CarId);
    }

    [Fact]
    public async Task AddAsync_AddsNewOrderItem()
    {
        // Arrange
        var context = GetDbContext();
        var service = new OrderItemService(context);

        var newItem = new OrderItem
        {
            OrderId = 1,
            CarId = 2,
            Quantity = 3
        };

        // Act
        await service.AddAsync(newItem);
        var items = await service.GetAllAsync();

        // Assert
        Assert.Contains(items, i => i.CarId == 2 && i.OrderId == 1 && i.Quantity == 3);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesOrderItem()
    {
        // Arrange
        var context = GetDbContext();
        var service = new OrderItemService(context);

        var item = await service.GetByIdAsync(1);
        item.Quantity = 5;

        // Act
        await service.UpdateAsync(item);
        var updatedItem = await service.GetByIdAsync(1);

        // Assert
        Assert.Equal(5, updatedItem.Quantity);
    }

    [Fact]
    public async Task DeleteAsync_RemovesOrderItem()
    {
        // Arrange
        var context = GetDbContext();
        var service = new OrderItemService(context);

        // Act
        await service.DeleteAsync(1);
        var item = await service.GetByIdAsync(1);

        // Assert
        Assert.Null(item);
    }
}
