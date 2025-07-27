using AutoShop.Controllers;
using AutoShop.Models;
using AutoShop.Services.Interfaces;
using AutoShop.ViewModels.Car;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

public class CarControllerTests
{
    private readonly Mock<ICarService> _carServiceMock;
    private readonly Mock<IWebHostEnvironment> _envMock;
    private readonly CarController _controller;

    public CarControllerTests()
    {
        _carServiceMock = new Mock<ICarService>();
        _envMock = new Mock<IWebHostEnvironment>();

        // Мокваме WebRootPath, като че ли имаме папка "wwwroot"
        _envMock.Setup(e => e.WebRootPath).Returns("C:\\FakeWebRoot");

        _controller = new CarController(_carServiceMock.Object, _envMock.Object);
    }

    [Fact]
    public async Task Index_ReturnsViewResult_WithCarQueryModel()
    {
        // Arrange
        _carServiceMock.Setup(s => s.GetAllAsync(null, 1, 5))
            .ReturnsAsync(new CarQueryModel
            {
                Cars = new List<CarViewModel>(),
                TotalCars = 0,
                CurrentPage = 1,
                CarsPerPage = 5
            });

        // Act
        var result = await _controller.Index(null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<CarQueryModel>(viewResult.Model);
        Assert.NotNull(model);
        Assert.IsAssignableFrom<IEnumerable<CarViewModel>>(model.Cars);
    }

    [Fact]
    public async Task Details_ReturnsNotFound_WhenCarIsNull()
    {
        // Arrange
        _carServiceMock.Setup(s => s.GetCarByIdAsync(1)).ReturnsAsync((Car)null);

        // Act
        var result = await _controller.Details(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_ReturnsViewResult_WithCar()
    {
        // Arrange
        var car = new Car { Id = 1, Model = "TestCar" };
        _carServiceMock.Setup(s => s.GetCarByIdAsync(1)).ReturnsAsync(car);

        // Act
        var result = await _controller.Details(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(car, viewResult.Model);
    }

    [Fact]
    public void Create_Get_ReturnsViewResult()
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
        _controller.ModelState.AddModelError("Model", "Required");
        var car = new Car();

        // Act
        var result = await _controller.Create(car, null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(car, viewResult.Model);
    }

    [Fact]
    public async Task Create_Post_ValidModel_NoImage_SavesCarAndRedirects()
    {
        // Arrange
        var car = new Car { Id = 1, Model = "Test" };

        // Act
        var result = await _controller.Create(car, null);

        // Assert
        _carServiceMock.Verify(s => s.AddCarAsync(car), Times.Once);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async Task Create_Post_ValidModel_WithImage_SavesFileAndCar()
    {
        // Arrange
        var car = new Car { Id = 1, Model = "Test" };

        var fileMock = new Mock<IFormFile>();
        var content = "Fake Image Content";
        var fileName = "test.jpg";

        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;

        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(ms.Length);
        fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
            .Returns<Stream, System.Threading.CancellationToken>((stream, token) =>
            {
                ms.CopyTo(stream);
                return Task.CompletedTask;
            });

        // Act
        var result = await _controller.Create(car, fileMock.Object);

        // Assert
        _carServiceMock.Verify(s => s.AddCarAsync(car), Times.Once);
        Assert.NotNull(car.ImageFileName);
        Assert.EndsWith(".jpg", car.ImageFileName);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async Task Edit_Get_ReturnsNotFound_WhenCarNotFound()
    {
        // Arrange
        _carServiceMock.Setup(s => s.GetCarByIdAsync(5)).ReturnsAsync((Car)null);

        // Act
        var result = await _controller.Edit(5);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Get_ReturnsViewResult_WithCar()
    {
        // Arrange
        var car = new Car { Id = 3, Model = "Car3" };
        _carServiceMock.Setup(s => s.GetCarByIdAsync(3)).ReturnsAsync(car);

        // Act
        var result = await _controller.Edit(3);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(car, viewResult.Model);
    }

    [Fact]
    public async Task Edit_Post_InvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        _controller.ModelState.AddModelError("Error", "Invalid");
        var car = new Car { Id = 2 };

        // Act
        var result = await _controller.Edit(car, null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(car, viewResult.Model);
    }

    [Fact]
    public async Task Edit_Post_ValidModel_NoImage_UpdatesCarAndRedirects()
    {
        // Arrange
        var car = new Car { Id = 10, Model = "Car10" };

        // Act
        var result = await _controller.Edit(car, null);

        // Assert
        _carServiceMock.Verify(s => s.UpdateCarAsync(car), Times.Once);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async Task Edit_Post_ValidModel_WithImage_UpdatesCarAndSetsImageName()
    {
        // Arrange
        var car = new Car { Id = 11 };

        var fileMock = new Mock<IFormFile>();
        var ms = new MemoryStream(new byte[] { 1, 2, 3 });
        fileMock.Setup(f => f.FileName).Returns("img.png");
        fileMock.Setup(f => f.Length).Returns(ms.Length);
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
            .Returns<Stream, System.Threading.CancellationToken>((stream, token) =>
            {
                ms.CopyTo(stream);
                return Task.CompletedTask;
            });

        // Act
        var result = await _controller.Edit(car, fileMock.Object);

        // Assert
        _carServiceMock.Verify(s => s.UpdateCarAsync(car), Times.Once);
        Assert.NotNull(car.ImageFileName);
        Assert.EndsWith(".png", car.ImageFileName);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async Task Delete_Get_ReturnsNotFound_WhenCarNotFound()
    {
        // Arrange
        _carServiceMock.Setup(s => s.GetCarByIdAsync(123)).ReturnsAsync((Car)null);

        // Act
        var result = await _controller.Delete(123);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_Get_ReturnsViewResult_WithCar()
    {
        // Arrange
        var car = new Car { Id = 7 };
        _carServiceMock.Setup(s => s.GetCarByIdAsync(7)).ReturnsAsync(car);

        // Act
        var result = await _controller.Delete(7);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(car, viewResult.Model);
    }

    [Fact]
    public async Task DeleteConfirmed_DeletesCarAndRedirects()
    {
        // Act
        var result = await _controller.DeleteConfirmed(9);

        // Assert
        _carServiceMock.Verify(s => s.DeleteCarAsync(9), Times.Once);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }
}
