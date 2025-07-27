using AutoShop.Areas.Admin.Controllers;
using AutoShop.Models;
using AutoShop.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class CategoryControllerTests
{
    private readonly Mock<ICategoryService> _categoryServiceMock;
    private readonly CategoryController _controller;

    public CategoryControllerTests()
    {
        _categoryServiceMock = new Mock<ICategoryService>();
        _controller = new CategoryController(_categoryServiceMock.Object);

        // Настройка на TempData за тестовете, където се използва
        var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        _controller.TempData = tempData;
    }

    [Fact]
    public async Task All_ReturnsViewWithCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Cat1" },
            new Category { Id = 2, Name = "Cat2" }
        };
        _categoryServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(categories);

        // Act
        var result = await _controller.All();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Category>>(viewResult.Model);
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
    public async Task Create_Post_InvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        _controller.ModelState.AddModelError("Name", "Required");
        var category = new Category();

        // Act
        var result = await _controller.Create(category);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(category, viewResult.Model);
    }

    [Fact]
    public async Task Create_Post_ValidModel_AddsCategoryAndRedirects()
    {
        // Arrange
        var category = new Category { Name = "New Category" };

        // Act
        var result = await _controller.Create(category);

        // Assert
        _categoryServiceMock.Verify(s => s.AddAsync(category), Times.Once);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(CategoryController.All), redirect.ActionName);
        Assert.True(_controller.TempData.ContainsKey("SuccessMessage"));
    }

    [Fact]
    public async Task Delete_CategoryNotFound_ReturnsJsonFailure()
    {
        // Arrange
        _categoryServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Category)null);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        dynamic data = jsonResult.Value;
        Assert.False(data.success);
        Assert.Equal("Категорията не беше намерена.", data.message);
    }

    [Fact]
    public async Task Delete_CategoryFound_DeletesAndReturnsJsonSuccess()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Cat1" };
        _categoryServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(category);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        _categoryServiceMock.Verify(s => s.DeleteAsync(1), Times.Once);
        var jsonResult = Assert.IsType<JsonResult>(result);
        dynamic data = jsonResult.Value;
        Assert.True(data.success);
        Assert.Equal($"Категорията '{category.Name}' беше изтрита.", data.message);
    }

    [Fact]
    public async Task Edit_Get_CategoryNotFound_ReturnsNotFound()
    {
        // Arrange
        _categoryServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Category)null);

        // Act
        var result = await _controller.Edit(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Категорията не е намерена.", notFoundResult.Value);
    }

    [Fact]
    public async Task Edit_Get_CategoryFound_ReturnsViewWithModel()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Cat1" };
        _categoryServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(category);

        // Act
        var result = await _controller.Edit(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(category, viewResult.Model);
    }

    [Fact]
    public async Task Edit_Post_InvalidModel_ReturnsBadRequestWithErrors()
    {
        // Arrange
        _controller.ModelState.AddModelError("Name", "Required");
        var category = new Category { Id = 1 };

        // Act
        var result = await _controller.Edit(category);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsAssignableFrom<IEnumerable<object>>(badRequestResult.Value);
        Assert.NotEmpty(errors);
    }

    [Fact]
    public async Task Edit_Post_CategoryNotFound_ReturnsNotFoundJson()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Cat1" };
        _categoryServiceMock.Setup(s => s.GetByIdAsync(category.Id)).ReturnsAsync((Category)null);

        // Act
        var result = await _controller.Edit(category);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        dynamic value = notFoundResult.Value;
        Assert.Equal("Категорията не съществува.", value.message);
    }

    [Fact]
    public async Task Edit_Post_ValidModel_UpdatesAndReturnsOkJson()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "NewName", Description = "NewDesc" };
        var existingCategory = new Category { Id = 1, Name = "OldName", Description = "OldDesc" };

        _categoryServiceMock.Setup(s => s.GetByIdAsync(category.Id)).ReturnsAsync(existingCategory);

        // Act
        var result = await _controller.Edit(category);

        // Assert
        _categoryServiceMock.Verify(s => s.UpdateAsync(existingCategory), Times.Once);
        Assert.Equal("NewName", existingCategory.Name);
        Assert.Equal("NewDesc", existingCategory.Description);

        var okResult = Assert.IsType<OkObjectResult>(result);
        dynamic value = okResult.Value;
        Assert.Equal("Категорията е обновена успешно.", value.message);
    }
}
