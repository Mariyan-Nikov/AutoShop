using AutoShop.Data;
using AutoShop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Linq;

public class ReviewServiceTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // различна база за всеки тест
            .Options;

        var context = new ApplicationDbContext(options);

        // Seed some cars
        context.Cars.Add(new Car { Id = 1, Brand = "BMW", Model = "X3", Year = 2021, Price = 35000m, RegistrationNumber = "B1234" });
        context.Cars.Add(new Car { Id = 2, Brand = "Audi", Model = "A4", Year = 2020, Price = 30000m, RegistrationNumber = "A1234" });
        context.SaveChanges();

        return context;
    }

    [Fact]
    public async Task AddReviewAsync_ShouldAddReview()
    {
        var context = GetDbContext();
        var service = new ReviewService(context);

        var review = new Review
        {
            CarId = 1,
            UserId = "user1",
            Content = "Great car!",
            CreatedOn = DateTime.UtcNow
        };

        await service.AddReviewAsync(review);

        var reviews = await service.GetAllReviewsAsync();

        reviews.Should().ContainSingle();
        reviews.First().Content.Should().Be("Great car!");
        reviews.First().CarId.Should().Be(1);
    }

    [Fact]
    public async Task GetReviewByIdAsync_ShouldReturnCorrectReview()
    {
        var context = GetDbContext();
        var service = new ReviewService(context);

        var review = new Review
        {
            CarId = 2,
            UserId = "user2",
            Content = "Nice drive.",
            CreatedOn = DateTime.UtcNow
        };

        await service.AddReviewAsync(review);

        var fetched = await service.GetReviewByIdAsync(review.Id);

        fetched.Should().NotBeNull();
        fetched!.Content.Should().Be("Nice drive.");
        fetched.CarId.Should().Be(2);
    }

    [Fact]
    public async Task UpdateReviewAsync_ShouldUpdateContent()
    {
        var context = GetDbContext();
        var service = new ReviewService(context);

        var review = new Review
        {
            CarId = 1,
            UserId = "user3",
            Content = "Old content",
            CreatedOn = DateTime.UtcNow
        };

        await service.AddReviewAsync(review);

        review.Content = "Updated content";

        await service.UpdateReviewAsync(review);

        var updated = await service.GetReviewByIdAsync(review.Id);

        updated!.Content.Should().Be("Updated content");
    }

    [Fact]
    public async Task DeleteReviewAsync_ShouldRemoveReview()
    {
        var context = GetDbContext();
        var service = new ReviewService(context);

        var review = new Review
        {
            CarId = 1,
            UserId = "user4",
            Content = "To be deleted",
            CreatedOn = DateTime.UtcNow
        };

        await service.AddReviewAsync(review);

        await service.DeleteReviewAsync(review.Id);

        var fetched = await service.GetReviewByIdAsync(review.Id);

        fetched.Should().BeNull();
    }

    [Fact]
    public void GetAllCars_ShouldReturnOrderedCars()
    {
        var context = GetDbContext();
        var service = new ReviewService(context);

        var cars = service.GetAllCars().ToList();

        cars.Should().HaveCount(2);
        cars[0].Brand.Should().Be("Audi");
        cars[1].Brand.Should().Be("BMW");
    }
}
