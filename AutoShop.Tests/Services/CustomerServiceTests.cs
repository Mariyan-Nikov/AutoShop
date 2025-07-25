using AutoShop.Data;
using AutoShop.Models;
using AutoShop.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class CustomerServiceTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);

        // Seed customers
        context.Customers.AddRange(
            new Customer { Id = 1, FullName = "John Doe", Email = "john@example.com", PhoneNumber = "1234567890" },
            new Customer { Id = 2, FullName = "Jane Smith", Email = "jane@example.com", PhoneNumber = "0987654321" }
        );
        context.SaveChanges();

        return context;
    }

    [Fact]
    public async Task GetAllCustomersAsync_ReturnsAllCustomers()
    {
        var context = GetDbContext();
        var service = new CustomerService(context);

        var customers = await service.GetAllCustomersAsync();

        Assert.NotNull(customers);
        Assert.Equal(2, customers.Count());
    }

    [Fact]
    public async Task GetCustomerByIdAsync_ReturnsCorrectCustomer()
    {
        var context = GetDbContext();
        var service = new CustomerService(context);

        var customer = await service.GetCustomerByIdAsync(1);

        Assert.NotNull(customer);
        Assert.Equal("John Doe", customer.FullName);
        Assert.Equal("john@example.com", customer.Email);
    }

    [Fact]
    public async Task AddCustomerAsync_AddsNewCustomer()
    {
        var context = GetDbContext();
        var service = new CustomerService(context);

        var newCustomer = new Customer
        {
            FullName = "Alice Johnson",
            Email = "alice@example.com",
            PhoneNumber = "1112223333"
        };

        await service.AddCustomerAsync(newCustomer);

        var customers = await service.GetAllCustomersAsync();

        Assert.Contains(customers, c => c.FullName == "Alice Johnson" && c.Email == "alice@example.com");
    }

    [Fact]
    public async Task UpdateCustomerAsync_UpdatesExistingCustomer()
    {
        var context = GetDbContext();
        var service = new CustomerService(context);

        var customer = await service.GetCustomerByIdAsync(2);
        customer.PhoneNumber = "9998887777";

        await service.UpdateCustomerAsync(customer);

        var updatedCustomer = await service.GetCustomerByIdAsync(2);

        Assert.Equal("9998887777", updatedCustomer.PhoneNumber);
    }

    [Fact]
    public async Task DeleteCustomerAsync_RemovesCustomer()
    {
        var context = GetDbContext();
        var service = new CustomerService(context);

        await service.DeleteCustomerAsync(1);

        var deletedCustomer = await service.GetCustomerByIdAsync(1);

        Assert.Null(deletedCustomer);
    }
}
