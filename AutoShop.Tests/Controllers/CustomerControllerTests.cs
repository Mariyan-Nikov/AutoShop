using AutoShop.Controllers;
using AutoShop.Models;
using AutoShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class CustomerControllerTests
{
    private readonly Mock<ICustomerService> _customerServiceMock;
    private readonly CustomerController _controller;

    public CustomerControllerTests()
    {
        _customerServiceMock = new Mock<ICustomerService>();
        _controller = new CustomerController(_customerServiceMock.Object);
    }

    [Fact]
    public async Task Index_ReturnsViewWithCustomers()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new Customer { Id = 1, FullName = "John Doe" },
            new Customer { Id = 2, FullName = "Jane Smith" }
        };
        _customerServiceMock.Setup(s => s.GetAllCustomersAsync()).ReturnsAsync(customers);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Customer>>(viewResult.Model);
        Assert.Equal(2, model.Count());
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
    public async Task Create_Post_ValidModel_RedirectsToIndex()
    {
        // Arrange
        var customer = new Customer { Id = 1, FullName = "John Doe", Email = "john@example.com", PhoneNumber = "123456789" };
        _customerServiceMock.Setup(s => s.AddCustomerAsync(customer)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(customer);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _customerServiceMock.Verify(s => s.AddCustomerAsync(customer), Times.Once);
    }

    [Fact]
    public async Task Create_Post_InvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        _controller.ModelState.AddModelError("Error", "Invalid data");
        var customer = new Customer();

        // Act
        var result = await _controller.Create(customer);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Customer>(viewResult.Model);
        Assert.Equal(customer, model);
    }

    [Fact]
    public async Task Edit_Get_ValidId_ReturnsViewWithCustomer()
    {
        // Arrange
        var customer = new Customer { Id = 1, FullName = "John Doe" };
        _customerServiceMock.Setup(s => s.GetCustomerByIdAsync(1)).ReturnsAsync(customer);

        // Act
        var result = await _controller.Edit(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(customer, viewResult.Model);
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
    public async Task Edit_Get_InvalidId_ReturnsNotFound()
    {
        // Arrange
        _customerServiceMock.Setup(s => s.GetCustomerByIdAsync(99)).ReturnsAsync((Customer?)null);

        // Act
        var result = await _controller.Edit(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_IdMismatch_ReturnsNotFound()
    {
        // Arrange
        var customer = new Customer { Id = 1 };

        // Act
        var result = await _controller.Edit(2, customer);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_ValidModel_RedirectsToIndex()
    {
        // Arrange
        var customer = new Customer { Id = 1 };
        _customerServiceMock.Setup(s => s.UpdateCustomerAsync(customer)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Edit(1, customer);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _customerServiceMock.Verify(s => s.UpdateCustomerAsync(customer), Times.Once);
    }

    [Fact]
    public async Task Edit_Post_InvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        _controller.ModelState.AddModelError("Error", "Invalid");
        var customer = new Customer { Id = 1 };

        // Act
        var result = await _controller.Edit(1, customer);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Customer>(viewResult.Model);
        Assert.Equal(customer, model);
    }

    [Fact]
    public async Task Delete_Get_ValidId_ReturnsViewWithCustomer()
    {
        // Arrange
        var customer = new Customer { Id = 1 };
        _customerServiceMock.Setup(s => s.GetCustomerByIdAsync(1)).ReturnsAsync(customer);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(customer, viewResult.Model);
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
    public async Task Delete_Get_InvalidId_ReturnsNotFound()
    {
        // Arrange
        _customerServiceMock.Setup(s => s.GetCustomerByIdAsync(99)).ReturnsAsync((Customer?)null);

        // Act
        var result = await _controller.Delete(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteConfirmed_Post_RedirectsToIndex()
    {
        // Arrange
        _customerServiceMock.Setup(s => s.DeleteCustomerAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteConfirmed(1);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _customerServiceMock.Verify(s => s.DeleteCustomerAsync(1), Times.Once);
    }
}
