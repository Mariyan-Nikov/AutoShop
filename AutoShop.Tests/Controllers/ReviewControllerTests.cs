using AutoShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class ReviewControllerTests
{
    private readonly Mock<IReviewService> _reviewServiceMock;
    private readonly ReviewController _controller;

    public ReviewControllerTests()
    {
        _reviewServiceMock = new Mock<IReviewService>();
        _controller = new ReviewController(_reviewServiceMock.Object);
    }

    [Fact]
    public async Task Index_ReturnsViewWithAllReviews()
    {
        var reviews = new List<Review>
        {
            new Review { Id = 1, Content = "Good" },
            new Review { Id = 2, Content = "Bad" }
        };
        _reviewServiceMock.Setup(s => s.GetAllReviewsAsync()).ReturnsAsync(reviews);

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Review>>(viewResult.Model);
        Assert.Equal(2, model.Count());
    }

    [Fact]
    public async Task Details_ExistingId_ReturnsViewWithReview()
    {
        var review = new Review { Id = 1, Content = "Test" };
        _reviewServiceMock.Setup(s => s.GetReviewByIdAsync(1)).ReturnsAsync(review);

        var result = await _controller.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(review, viewResult.Model);
    }

    [Fact]
    public async Task Details_NonExistingId_ReturnsNotFound()
    {
        _reviewServiceMock.Setup(s => s.GetReviewByIdAsync(99)).ReturnsAsync((Review?)null);

        var result = await _controller.Details(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Create_Get_ReturnsViewWithReviewAndCarsInViewBag()
    {
        var cars = new List<Car>
        {
            new Car { Id = 5, Brand = "BrandA" },
            new Car { Id = 6, Brand = "BrandB" }
        };
        _reviewServiceMock.Setup(s => s.GetAllCars()).Returns(cars);

        var result = _controller.Create(5);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Review>(viewResult.Model);
        Assert.Equal(5, model.CarId);

        var carsSelectList = Assert.IsType<SelectList>(viewResult.ViewData["Cars"]);
        Assert.Equal(2, carsSelectList.Count());
        Assert.Equal(5, carsSelectList.SelectedValue);
    }

    [Fact]
    public async Task Create_Post_ValidModel_AddsReviewAndRedirects()
    {
        var review = new Review { CarId = 5, Content = "Nice" };
        _reviewServiceMock.Setup(s => s.AddReviewAsync(review)).Returns(Task.CompletedTask);

        var result = await _controller.Create(review);

        _reviewServiceMock.Verify(s => s.AddReviewAsync(It.Is<Review>(r => r == review && r.CreatedOn != default)), Times.Once);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public async Task Create_Post_InvalidModel_ReturnsViewWithModelAndCars()
    {
        _controller.ModelState.AddModelError("Error", "Invalid model");
        var review = new Review { CarId = 5 };
        var cars = new List<Car>
        {
            new Car { Id = 5, Brand = "BrandA" }
        };
        _reviewServiceMock.Setup(s => s.GetAllCars()).Returns(cars);

        var result = await _controller.Create(review);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(review, viewResult.Model);

        var carsSelectList = Assert.IsType<SelectList>(viewResult.ViewData["Cars"]);
        Assert.Single(carsSelectList);
        Assert.Equal(5, carsSelectList.SelectedValue);
    }

    [Fact]
    public async Task Edit_Get_ExistingId_ReturnsViewWithReviewAndCars()
    {
        var review = new Review { Id = 1, CarId = 5 };
        var cars = new List<Car>
        {
            new Car { Id = 5, Brand = "BrandA" },
            new Car { Id = 6, Brand = "BrandB" }
        };
        _reviewServiceMock.Setup(s => s.GetReviewByIdAsync(1)).ReturnsAsync(review);
        _reviewServiceMock.Setup(s => s.GetAllCars()).Returns(cars);

        var result = await _controller.Edit(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(review, viewResult.Model);

        var carsSelectList = Assert.IsType<SelectList>(viewResult.ViewData["Cars"]);
        Assert.Equal(2, carsSelectList.Count());
        Assert.Equal(5, carsSelectList.SelectedValue);
    }

    [Fact]
    public async Task Edit_Get_NonExistingId_ReturnsNotFound()
    {
        _reviewServiceMock.Setup(s => s.GetReviewByIdAsync(99)).ReturnsAsync((Review?)null);

        var result = await _controller.Edit(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_IdMismatch_ReturnsBadRequest()
    {
        var review = new Review { Id = 1 };

        var result = await _controller.Edit(2, review);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Edit_Post_ValidModel_UpdatesReviewAndRedirects()
    {
        var review = new Review { Id = 1, CarId = 5 };
        _reviewServiceMock.Setup(s => s.UpdateReviewAsync(review)).Returns(Task.CompletedTask);

        var result = await _controller.Edit(1, review);

        _reviewServiceMock.Verify(s => s.UpdateReviewAsync(review), Times.Once);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public async Task Edit_Post_InvalidModel_ReturnsViewWithModelAndCars()
    {
        _controller.ModelState.AddModelError("Error", "Invalid");
        var review = new Review { Id = 1, CarId = 5 };
        var cars = new List<Car>
        {
            new Car { Id = 5, Brand = "BrandA" }
        };
        _reviewServiceMock.Setup(s => s.GetAllCars()).Returns(cars);

        var result = await _controller.Edit(1, review);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(review, viewResult.Model);

        var carsSelectList = Assert.IsType<SelectList>(viewResult.ViewData["Cars"]);
        Assert.Single(carsSelectList);
        Assert.Equal(5, carsSelectList.SelectedValue);
    }

    [Fact]
    public async Task Delete_Get_ExistingId_ReturnsViewWithReview()
    {
        var review = new Review { Id = 1 };
        _reviewServiceMock.Setup(s => s.GetReviewByIdAsync(1)).ReturnsAsync(review);

        var result = await _controller.Delete(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(review, viewResult.Model);
    }

    [Fact]
    public async Task Delete_Get_NonExistingId_ReturnsNotFound()
    {
        _reviewServiceMock.Setup(s => s.GetReviewByIdAsync(99)).ReturnsAsync((Review?)null);

        var result = await _controller.Delete(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteConfirmed_Post_DeletesReviewAndRedirects()
    {
        _reviewServiceMock.Setup(s => s.DeleteReviewAsync(1)).Returns(Task.CompletedTask);

        var result = await _controller.DeleteConfirmed(1);

        _reviewServiceMock.Verify(s => s.DeleteReviewAsync(1), Times.Once);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }
}
