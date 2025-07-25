using AutoShop.Data;
using AutoShop.Models;
using AutoShop.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class OrderServiceTests
{
    private readonly ApplicationDbContext dbContext;
    private readonly OrderService orderService;

    public OrderServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        dbContext = new ApplicationDbContext(options);
        orderService = new OrderService(dbContext);
    }

    [Fact]
    public async Task AddOrderAsync_ShouldAddOrderToDatabase()
    {
        // Arrange
        var order = new Order
        {
            CustomerId = 1,
            OrderDate = DateTime.UtcNow,
            Status = "Pending"
        };

        // Act
        await orderService.AddOrderAsync(order);

        // Assert
        var orderInDb = await dbContext.Orders.FirstOrDefaultAsync();
        orderInDb.Should().NotBeNull();
        orderInDb!.CustomerId.Should().Be(1);
        orderInDb.Status.Should().Be("Pending");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllOrders()
    {
        // Arrange
        dbContext.Orders.AddRange(new[]
        {
            new Order { CustomerId = 1, OrderDate = DateTime.UtcNow },
            new Order { CustomerId = 2, OrderDate = DateTime.UtcNow }
        });
        await dbContext.SaveChangesAsync();

        // Act
        var result = await orderService.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetOrderByIdAsync_ShouldReturnCorrectOrder()
    {
        // Arrange
        var order = new Order { CustomerId = 1, OrderDate = DateTime.UtcNow };
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await orderService.GetOrderByIdAsync(order.Id);

        // Assert
        result.Should().NotBeNull();
        result!.CustomerId.Should().Be(1);
    }

    [Fact]
    public async Task UpdateOrderAsync_ShouldModifyOrder()
    {
        // Arrange
        var order = new Order { CustomerId = 1, OrderDate = DateTime.UtcNow, Status = "Pending" };
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();

        // Modify
        order.Status = "Completed";

        // Act
        await orderService.UpdateOrderAsync(order);

        // Assert
        var orderInDb = await dbContext.Orders.FindAsync(order.Id);
        orderInDb!.Status.Should().Be("Completed");
    }

    [Fact]
    public async Task DeleteOrderAsync_ShouldRemoveOrder()
    {
        // Arrange
        var order = new Order { CustomerId = 1, OrderDate = DateTime.UtcNow };
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();

        // Act
        await orderService.DeleteOrderAsync(order.Id);

        // Assert
        var orderInDb = await dbContext.Orders.FindAsync(order.Id);
        orderInDb.Should().BeNull();
    }
}
