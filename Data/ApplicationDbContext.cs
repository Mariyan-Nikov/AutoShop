using AutoShop.Data.Entities;
using AutoShop.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoShop.Data
{
    // Основният контекст за базата данни, наследяващ IdentityDbContext за управление на потребители и роли
    public class ApplicationDbContext : IdentityDbContext
    {
        // Конструктор, приемащ конфигурационни опции
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet за таблицата с коли
        public DbSet<Car> Cars { get; set; } = null!;

        // DbSet за таблицата с поръчки
        public DbSet<Order> Orders { get; set; } = null!;

        // DbSet за таблицата с ревюта
        public DbSet<Review> Reviews { get; set; } = null!;

        // DbSet за таблицата с дилъри 
        public DbSet<Dealer> Dealers { get; set; } = null!;

        // DbSet за таблицата с категории
        public DbSet<Category> Categories { get; set; } = null!;

        // DbSet за таблицата с настройки
        public DbSet<Setting> Settings { get; set; } = null!;

        // DbSet за таблицата с документи за поръчки (заявки)
        public DbSet<OrderDocument> OrderDocuments { get; set; } = null!;
    }
}
