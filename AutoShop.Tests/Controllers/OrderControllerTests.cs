using AutoShop.Controllers;
using AutoShop.Models;
using AutoShop.Services.Interfaces;
using AutoShop.ViewModels.OrderDocument;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class OrderControllerTests
{
    private readonly Mock<IOrderService> _orderServiceMock;
    private readonly OrderController _controller;

    public OrderControllerTests()
    {
        _orderServiceMock = new Mock<IOrderService>();
        _controller = new OrderController(_orderServiceMock.Object);

        // Setup TempData, за да избегнем NullReferenceException
        var tempData = new TempDataDictionary(
            new DefaultHttpContext(),
            Mock.Of<ITempDataProvider>());
        _controller.TempData = tempData;
    }

    [Fact]
    public async Task Index_ReturnsViewWithOrders()
    {
        // Arrange
        var orders = new List<Order> { new Order { Id = 1 }, new Order { Id = 2 } };
        _orderServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(orders);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Order>>(viewResult.Model);
        Assert.Equal(2, ((List<Order>)model).Count);
    }

    [Fact]
    public void Create_Get_ReturnsView()
    {
        // Act
        var result = _controller.Create();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Create_Post_InvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        _controller.ModelState.AddModelError("Test", "Error");
        var order = new Order();

        // Act
        var result = await _controller.Create(order);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(order, viewResult.Model);
    }

    [Fact]
    public async Task Create_Post_ValidModel_CallsAddOrderAndRedirects()
    {
        // Arrange
        var order = new Order();

        // Act
        var result = await _controller.Create(order);

        // Assert
        _orderServiceMock.Verify(s => s.AddOrderAsync(order), Times.Once);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(OrderController.Index), redirect.ActionName);
    }

    [Fact]
    public async Task Edit_Get_NullId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.Edit(null);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Get_OrderNotFound_ReturnsNotFound()
    {
        // Arrange
        _orderServiceMock.Setup(s => s.GetOrderByIdAsync(It.IsAny<int>())).ReturnsAsync((Order)null);

        // Act
        var result = await _controller.Edit(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Get_OrderFound_ReturnsViewWithModel()
    {
        // Arrange
        var order = new Order { Id = 1 };
        _orderServiceMock.Setup(s => s.GetOrderByIdAsync(1)).ReturnsAsync(order);

        // Act
        var result = await _controller.Edit(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(order, viewResult.Model);
    }

    [Fact]
    public async Task Edit_Post_IdMismatch_ReturnsNotFound()
    {
        // Arrange
        var order = new Order { Id = 2 };

        // Act
        var result = await _controller.Edit(1, order);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_InvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        var order = new Order { Id = 1 };
        _controller.ModelState.AddModelError("Test", "Error");

        // Act
        var result = await _controller.Edit(1, order);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(order, viewResult.Model);
    }

    [Fact]
    public async Task Edit_Post_ValidModel_UpdatesAndRedirects()
    {
        // Arrange
        var order = new Order { Id = 1 };

        // Act
        var result = await _controller.Edit(1, order);

        // Assert
        _orderServiceMock.Verify(s => s.UpdateOrderAsync(order), Times.Once);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(OrderController.Index), redirect.ActionName);
    }

    [Fact]
    public async Task Delete_Get_NullId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.Delete(null);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_Get_OrderNotFound_ReturnsNotFound()
    {
        // Arrange
        _orderServiceMock.Setup(s => s.GetOrderByIdAsync(It.IsAny<int>())).ReturnsAsync((Order)null);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_Get_OrderFound_ReturnsViewWithModel()
    {
        // Arrange
        var order = new Order { Id = 1 };
        _orderServiceMock.Setup(s => s.GetOrderByIdAsync(1)).ReturnsAsync(order);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(order, viewResult.Model);
    }

    [Fact]
    public async Task DeleteConfirmed_DeletesAndRedirects()
    {
        // Act
        var result = await _controller.DeleteConfirmed(1);

        // Assert
        _orderServiceMock.Verify(s => s.DeleteOrderAsync(1), Times.Once);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(OrderController.Index), redirect.ActionName);
    }

    [Fact]
    public void OrderDocumentGet_ReturnsViewWithModel()
    {
        // Act
        var result = _controller.OrderDocumentGet(5);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<OrderDocumentViewModel>(viewResult.Model);
        Assert.Equal(5, model.CarId);
    }

    [Fact]
    public void OrderDocumentPost_InvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        var model = new OrderDocumentViewModel();
        _controller.ModelState.AddModelError("Test", "Error");

        // Act
        var result = _controller.OrderDocumentPost(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
    }

    [Fact]
    public void OrderDocumentPost_ValidModel_SetsTempDataAndRedirects()
    {
        // Arrange
        var model = new OrderDocumentViewModel();

        // TempData е вече настроено в конструктора

        // Act
        var result = _controller.OrderDocumentPost(model);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Home", redirect.ControllerName);
        Assert.True(_controller.TempData.ContainsKey("Message"));
    }
}
