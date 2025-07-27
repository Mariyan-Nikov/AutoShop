using AutoShop.Data.Entities;
using AutoShop.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoShop.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Car> Cars { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<Dealer> Dealers { get; set; }
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Setting> Settings { get; set; } = null!;
        public DbSet<OrderDocument> OrderDocuments { get; set; }


    }
}
