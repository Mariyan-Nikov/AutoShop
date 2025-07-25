using AutoShop.Controllers;
using AutoShop.Models;
using AutoShop.Services.Interfaces;
using AutoShop.ViewModels.Car;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class OrderItemControllerTests
{
    private Mock<IOrderItemService> mockOrderItemService;
    private Mock<ICarService> mockCarService;
    private Mock<IOrderService> mockOrderService;
    private OrderItemController controller;

    public OrderItemControllerTests()
    {
        mockOrderItemService = new Mock<IOrderItemService>();
        mockCarService = new Mock<ICarService>();
        mockOrderService = new Mock<IOrderService>();

        // Мокване на CarService.GetAllAsync()
        mockCarService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(new List<CarViewModel>
            {
                new CarViewModel { Id = 1, Brand = "BMW", Model = "X5", Year = 2020, RegistrationNumber = "1234AB" },
                new CarViewModel { Id = 2, Brand = "Audi", Model = "A4", Year = 2019, RegistrationNumber = "5678CD" }
            });

        // Мокване на OrderService.GetAllAsync()
        mockOrderService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(new List<Order>
            {
                new Order { Id = 1 },
                new Order { Id = 2 }
            });

        controller = new OrderItemController(
            mockOrderItemService.Object,
            mockCarService.Object,
            mockOrderService.Object);
    }

    [Fact]
    public async Task Index_ReturnsViewWithAllOrderItems()
    {
        // Arrange
        mockOrderItemService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(new List<OrderItem>
            {
                new OrderItem { Id = 1, CarId = 1, OrderId = 1 },
                new OrderItem { Id = 2, CarId = 2, OrderId = 2 }
            });

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<OrderItem>>(viewResult.Model);
        Assert.Equal(2, model.Count());
    }

    [Fact]
    public async Task Details_WithExistingId_ReturnsViewWithOrderItem()
    {
        var orderItem = new OrderItem { Id = 1, CarId = 1, OrderId = 1 };
        mockOrderItemService.Setup(s => s.GetByIdAsync(1))
            .ReturnsAsync(orderItem);

        var result = await controller.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<OrderItem>(viewResult.Model);
        Assert.Equal(1, model.Id);
    }

    [Fact]
    public async Task Details_WithNonExistingId_ReturnsNotFound()
    {
        mockOrderItemService.Setup(s => s.GetByIdAsync(1))
            .ReturnsAsync((OrderItem?)null);

        var result = await controller.Details(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateGet_ReturnsViewWithSelectLists()
    {
        var result = await controller.Create();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(viewResult.ViewData["Cars"]);
        Assert.NotNull(viewResult.ViewData["Orders"]);
    }

    [Fact]
    public async Task CreatePost_InvalidModel_ReturnsViewWithSelectLists()
    {
        controller.ModelState.AddModelError("Test", "Error");

        var item = new OrderItem();

        var result = await controller.Create(item);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(viewResult.ViewData["Cars"]);
        Assert.NotNull(viewResult.ViewData["Orders"]);
        Assert.Equal(item, viewResult.Model);
    }

    [Fact]
    public async Task CreatePost_ValidModel_RedirectsToIndex()
    {
        var item = new OrderItem { Id = 1, CarId = 1, OrderId = 1 };

        var result = await controller.Create(item);

        mockOrderItemService.Verify(s => s.AddAsync(item), Times.Once);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public async Task EditGet_WithExistingId_ReturnsViewWithOrderItemAndSelectLists()
    {
        var item = new OrderItem { Id = 1, CarId = 1, OrderId = 1 };
        mockOrderItemService.Setup(s => s.GetByIdAsync(1))
            .ReturnsAsync(item);

        var result = await controller.Edit(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(viewResult.ViewData["Cars"]);
        Assert.NotNull(viewResult.ViewData["Orders"]);
        var model = Assert.IsType<OrderItem>(viewResult.Model);
        Assert.Equal(1, model.Id);
    }

    [Fact]
    public async Task EditGet_WithNonExistingId_ReturnsNotFound()
    {
        mockOrderItemService.Setup(s => s.GetByIdAsync(1))
            .ReturnsAsync((OrderItem?)null);

        var result = await controller.Edit(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task EditPost_IdMismatch_ReturnsBadRequest()
    {
        var item = new OrderItem { Id = 2, CarId = 1, OrderId = 1 };

        var result = await controller.Edit(1, item);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task EditPost_InvalidModel_ReturnsViewWithSelectLists()
    {
        controller.ModelState.AddModelError("Test", "Error");
        var item = new OrderItem { Id = 1, CarId = 1, OrderId = 1 };

        var result = await controller.Edit(1, item);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(viewResult.ViewData["Cars"]);
        Assert.NotNull(viewResult.ViewData["Orders"]);
        Assert.Equal(item, viewResult.Model);
    }

    [Fact]
    public async Task EditPost_ValidModel_RedirectsToIndex()
    {
        var item = new OrderItem { Id = 1, CarId = 1, OrderId = 1 };

        var result = await controller.Edit(1, item);

        mockOrderItemService.Verify(s => s.UpdateAsync(item), Times.Once);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public async Task DeleteGet_WithExistingId_ReturnsViewWithOrderItem()
    {
        var item = new OrderItem { Id = 1, CarId = 1, OrderId = 1 };
        mockOrderItemService.Setup(s => s.GetByIdAsync(1))
            .ReturnsAsync(item);

        var result = await controller.Delete(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<OrderItem>(viewResult.Model);
        Assert.Equal(1, model.Id);
    }

    [Fact]
    public async Task DeleteGet_WithNonExistingId_ReturnsNotFound()
    {
        mockOrderItemService.Setup(s => s.GetByIdAsync(1))
            .ReturnsAsync((OrderItem?)null);

        var result = await controller.Delete(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteConfirmed_Post_RedirectsToIndex()
    {
        var result = await controller.DeleteConfirmed(1);

        mockOrderItemService.Verify(s => s.DeleteAsync(1), Times.Once);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }
}
