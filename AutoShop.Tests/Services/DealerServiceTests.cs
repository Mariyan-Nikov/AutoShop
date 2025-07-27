using AutoShop.Data;
using AutoShop.Models;
using AutoShop.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class DealerServiceTests
{
    private ApplicationDbContext GetInMemoryDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new ApplicationDbContext(options);
    }

    private Dealer CreateDealer(int id = 0, string name = "Dealer")
    {
        return new Dealer
        {
            Id = id,
            Name = name,
            Address = "Test Address",
            Email = "test@example.com",
            PhoneNumber = "1234567890"
        };
    }

    [Fact]
    public async Task AddDealerAsync_AddsDealerSuccessfully()
    {
        using var context = GetInMemoryDbContext(Guid.NewGuid().ToString());
        var service = new DealerService(context);

        var dealer = CreateDealer(name: "New Dealer");

        await service.AddDealerAsync(dealer);

        var dealers = await context.Dealers.ToListAsync();

        Assert.Single(dealers);
        Assert.Equal("New Dealer", dealers[0].Name);
    }

    [Fact]
    public async Task GetAllDealersAsync_ReturnsAllDealers()
    {
        using var context = GetInMemoryDbContext(Guid.NewGuid().ToString());
        context.Dealers.Add(CreateDealer(id: 1, name: "Dealer 1"));
        context.Dealers.Add(CreateDealer(id: 2, name: "Dealer 2"));
        await context.SaveChangesAsync();

        var service = new DealerService(context);

        var dealers = await service.GetAllDealersAsync();

        Assert.Equal(2, dealers.Count());
    }

    [Fact]
    public async Task GetDealerByIdAsync_ReturnsCorrectDealer()
    {
        using var context = GetInMemoryDbContext(Guid.NewGuid().ToString());
        var dealer = CreateDealer(id: 1, name: "Dealer 1");
        context.Dealers.Add(dealer);
        await context.SaveChangesAsync();

        var service = new DealerService(context);

        var foundDealer = await service.GetDealerByIdAsync(1);

        Assert.NotNull(foundDealer);
        Assert.Equal("Dealer 1", foundDealer.Name);
    }

    [Fact]
    public async Task UpdateDealerAsync_UpdatesDealerSuccessfully()
    {
        using var context = GetInMemoryDbContext(Guid.NewGuid().ToString());
        var dealer = CreateDealer(id: 1, name: "Dealer 1");
        context.Dealers.Add(dealer);
        await context.SaveChangesAsync();

        var service = new DealerService(context);

        dealer.Name = "Updated Dealer";
        await service.UpdateDealerAsync(dealer);

        var updatedDealer = await context.Dealers.FindAsync(1);

        Assert.Equal("Updated Dealer", updatedDealer.Name);
    }

    [Fact]
    public async Task DeleteDealerAsync_DeletesDealerSuccessfully()
    {
        using var context = GetInMemoryDbContext(Guid.NewGuid().ToString());
        var dealer = CreateDealer(id: 1, name: "Dealer 1");
        context.Dealers.Add(dealer);
        await context.SaveChangesAsync();

        var service = new DealerService(context);

        await service.DeleteDealerAsync(1);

        var dealers = await context.Dealers.ToListAsync();

        Assert.Empty(dealers);
    }

    [Fact]
    public async Task DeleteDealerAsync_WhenDealerDoesNotExist_DoesNothing()
    {
        using var context = GetInMemoryDbContext(Guid.NewGuid().ToString());

        var service = new DealerService(context);

        // Should not throw or delete anything
        await service.DeleteDealerAsync(999);

        var dealers = await context.Dealers.ToListAsync();

        Assert.Empty(dealers);
    }

    [Fact]
    public async Task DealerExistsAsync_ReturnsTrueIfExists()
    {
        using var context = GetInMemoryDbContext(Guid.NewGuid().ToString());
        var dealer = CreateDealer(id: 1, name: "Dealer 1");
        context.Dealers.Add(dealer);
        await context.SaveChangesAsync();

        var service = new DealerService(context);

        var exists = await service.DealerExistsAsync(1);

        Assert.True(exists);
    }

    [Fact]
    public async Task DealerExistsAsync_ReturnsFalseIfNotExists()
    {
        using var context = GetInMemoryDbContext(Guid.NewGuid().ToString());

        var service = new DealerService(context);

        var exists = await service.DealerExistsAsync(123);

        Assert.False(exists);
    }
}
