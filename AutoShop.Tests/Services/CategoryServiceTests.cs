using System;
using System.Linq;
using System.Threading.Tasks;
using AutoShop.Data;
using AutoShop.Models;
using AutoShop.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class CategoryServiceTests
{
    private ApplicationDbContext GetInMemoryDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCategories()
    {
        // Arrange
        using var context = GetInMemoryDbContext(Guid.NewGuid().ToString());
        context.Categories.Add(new Category { Name = "Category1" });
        context.Categories.Add(new Category { Name = "Category2" });
        await context.SaveChangesAsync();

        var service = new CategoryService(context);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.Name == "Category1");
        Assert.Contains(result, c => c.Name == "Category2");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectCategory()
    {
        // Arrange
        using var context = GetInMemoryDbContext(Guid.NewGuid().ToString());
        var category = new Category { Name = "Category1" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = new CategoryService(context);

        // Act
        var result = await service.GetByIdAsync(category.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(category.Name, result.Name);
    }

    [Fact]
    public async Task AddAsync_AddsCategorySuccessfully()
    {
        // Arrange
        using var context = GetInMemoryDbContext(Guid.NewGuid().ToString());
        var service = new CategoryService(context);
        var newCategory = new Category { Name = "NewCategory" };

        // Act
        await service.AddAsync(newCategory);

        // Assert
        var categories = await context.Categories.ToListAsync();
        Assert.Single(categories);
        Assert.Equal("NewCategory", categories.First().Name);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesCategorySuccessfully()
    {
        // Arrange
        using var context = GetInMemoryDbContext(Guid.NewGuid().ToString());
        var category = new Category { Name = "OldName" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = new CategoryService(context);

        // Act
        category.Name = "UpdatedName";
        await service.UpdateAsync(category);

        // Assert
        var updatedCategory = await context.Categories.FindAsync(category.Id);
        Assert.NotNull(updatedCategory);
        Assert.Equal("UpdatedName", updatedCategory.Name);
    }

    [Fact]
    public async Task DeleteAsync_DeletesCategorySuccessfully()
    {
        // Arrange
        using var context = GetInMemoryDbContext(Guid.NewGuid().ToString());
        var category = new Category { Name = "ToDelete" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = new CategoryService(context);

        // Act
        await service.DeleteAsync(category.Id);

        // Assert
        var categories = await context.Categories.ToListAsync();
        Assert.Empty(categories);
    }

    [Fact]
    public async Task DeleteAsync_WhenCategoryDoesNotExist_DoesNothing()
    {
        // Arrange
        using var context = GetInMemoryDbContext(Guid.NewGuid().ToString());
        var service = new CategoryService(context);

        // Act
        await service.DeleteAsync(999); // Несъществуващ ID

        // Assert
        var categories = await context.Categories.ToListAsync();
        Assert.Empty(categories);
    }
}
