using AutoShop.Data;
using AutoShop.Hubs;
using AutoShop.Services;
using AutoShop.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Конфигурация на връзка към база данни
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// 2. Регистрация на DbContext с SQL 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Регистрация на Identity с роли
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 4. Регистрация на услугите
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IDealerService, DealerService>();
builder.Services.AddScoped<IOrderDocumentService, OrderDocumentService>();

// 5. Добавяне на MVC, Razor Pages и Blazor Server
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor(); // Важна добавка за Blazor Server

// 6. Добавяне на SignalR хъб
builder.Services.AddSignalR();

var app = builder.Build();

// 7. Seed роли и първи администратор
await SeedRolesAndAdminUser(app.Services);

app.UseHttpsRedirection(); // Ако не ползваш HTTPS, можеш да го коментираш

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 8. Добавяне на маршрут за Blazor Server хъб
app.MapBlazorHub();
app.MapHub<CarHub>("/carHub");

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/500");
    app.UseHsts();
}

// Обработка на статус кодове
app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.Run();

static async Task SeedRolesAndAdminUser(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string[] roles = { "Administrator", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    var firstUser = userManager.Users.FirstOrDefault();

    if (firstUser != null)
    {
        var isAdmin = await userManager.IsInRoleAsync(firstUser, "Administrator");
        if (!isAdmin)
        {
            await userManager.AddToRoleAsync(firstUser, "Administrator");
        }
    }
}
