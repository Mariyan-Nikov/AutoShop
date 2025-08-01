using AutoShop.Controllers;
using AutoShop.Hubs;
using AutoShop.Models;
using AutoShop.Services.Interfaces;
using AutoShop.ViewModels.Car;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

public class CarControllerTests
{
    private readonly Mock<ICarService> _carServiceMock;
    private readonly Mock<IWebHostEnvironment> _envMock;
    private readonly Mock<IHubContext<CarHub>> _hubContextMock;
    private readonly Mock<IClientProxy> _clientProxyMock;
    private readonly CarController _controller;

    public CarControllerTests()
    {
        _carServiceMock = new Mock<ICarService>();
        _envMock = new Mock<IWebHostEnvironment>();
        _hubContextMock = new Mock<IHubContext<CarHub>>();
        _clientProxyMock = new Mock<IClientProxy>();

        // Setup WebRootPath for image saving
        _envMock.Setup(e => e.WebRootPath).Returns("C:\\FakeWebRoot");

        // Setup SignalR Clients.All.SendAsync
        var clientsMock = new Mock<IHubClients>();
        clientsMock.Setup(c => c.All).Returns(_clientProxyMock.Object);
        _hubContextMock.Setup(h => h.Clients).Returns(clientsMock.Object);

        _controller = new CarController(_carServiceMock.Object, _envMock.Object, _hubContextMock.Object);
    }

    [Fact]
    public async Task Index_ReturnsView_WithCars()
    {
        _carServiceMock.Setup(s => s.GetAllAsync("", 1, 10))
            .ReturnsAsync(new CarQueryModel
            {
                Cars = new List<CarViewModel>(),
                TotalCars = 0,
                CurrentPage = 1,
                CarsPerPage = 10
            });

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(viewResult.Model);
        Assert.IsAssignableFrom<CarQueryModel>(viewResult.Model);
    }

    [Fact]
    public void Create_Get_ReturnsView()
    {
        var result = _controller.Create();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Create_Post_InvalidModel_ReturnsViewWithModel()
    {
        _controller.ModelState.AddModelError("Brand", "Required");
        var car = new Car();

        var result = await _controller.Create(car, null);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(car, viewResult.Model);
    }

    [Fact]
    public async Task Create_Post_ValidModel_WithImage_SavesCarAndSendsSignalRNotification()
    {
        var car = new Car { Brand = "BMW", Model = "X5" };

        // Mock IFormFile with some content
        var fileMock = new Mock<IFormFile>();
        var content = "Fake image content";
        var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
        fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
        fileMock.Setup(f => f.FileName).Returns("test.jpg");
        fileMock.Setup(f => f.Length).Returns(ms.Length);

        var result = await _controller.Create(car, fileMock.Object);

        _carServiceMock.Verify(s => s.AddCarAsync(It.IsAny<Car>()), Times.Once);

        // Проверка, че SignalR SendAsync е извикан
        _clientProxyMock.Verify(
            client => client.SendCoreAsync(
                "ReceiveCarNotification",
                It.Is<object[]>(o => o != null && o.Length == 1 && o[0].ToString() == "BMW X5"),
                default),
            Times.Once);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public async Task Details_ReturnsNotFound_WhenCarNull()
    {
        _carServiceMock.Setup(s => s.GetCarByIdAsync(1)).ReturnsAsync((Car)null);

        var result = await _controller.Details(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_ReturnsView_WithCar()
    {
        var car = new Car { Id = 1, Brand = "Audi" };
        _carServiceMock.Setup(s => s.GetCarByIdAsync(1)).ReturnsAsync(car);

        var result = await _controller.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(car, viewResult.Model);
    }

    [Fact]
    public async Task Edit_Get_ReturnsNotFound_WhenCarNull()
    {
        _carServiceMock.Setup(s => s.GetCarByIdAsync(42)).ReturnsAsync((Car)null);

        var result = await _controller.Edit(42);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Get_ReturnsView_WithCar()
    {
        var car = new Car { Id = 2, Brand = "Tesla" };
        _carServiceMock.Setup(s => s.GetCarByIdAsync(2)).ReturnsAsync(car);

        var result = await _controller.Edit(2);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(car, viewResult.Model);
    }

    [Fact]
    public async Task Edit_Post_InvalidModel_ReturnsViewWithModel()
    {
        _controller.ModelState.AddModelError("Error", "Invalid");
        var car = new Car();

        var result = await _controller.Edit(car);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(car, viewResult.Model);
    }

    [Fact]
    public async Task Edit_Post_ValidModel_UpdatesCarAndRedirects()
    {
        var car = new Car { Id = 5, Brand = "Ford" };

        var result = await _controller.Edit(car);

        _carServiceMock.Verify(s => s.UpdateCarAsync(car), Times.Once);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public async Task Delete_Get_ReturnsNotFound_WhenCarNull()
    {
        _carServiceMock.Setup(s => s.GetCarByIdAsync(99)).ReturnsAsync((Car)null);

        var result = await _controller.Delete(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_Get_ReturnsView_WithCar()
    {
        var car = new Car { Id = 7 };
        _carServiceMock.Setup(s => s.GetCarByIdAsync(7)).ReturnsAsync(car);

        var result = await _controller.Delete(7);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(car, viewResult.Model);
    }

    [Fact]
    public async Task DeleteConfirmed_DeletesCarAndRedirects()
    {
        var result = await _controller.DeleteConfirmed(7);

        _carServiceMock.Verify(s => s.DeleteCarAsync(7), Times.Once);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }
}
