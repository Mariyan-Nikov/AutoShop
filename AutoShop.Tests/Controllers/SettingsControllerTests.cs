using AutoShop.Areas.Admin.Controllers;
using AutoShop.Data;
using AutoShop.Data.Entities;
using AutoShop.ViewModels.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Threading.Tasks;
using Xunit;

public class SettingsControllerTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private void SetupTempData(Controller controller)
    {
        var tempData = new TempDataDictionary(
            new DefaultHttpContext(),
            Mock.Of<ITempDataProvider>());

        controller.TempData = tempData;
    }

    [Fact]
    public async Task Index_Get_ReturnsViewWithDefaultSettings()
    {
        var context = GetInMemoryDbContext();
        var controller = new SettingsController(context);

        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<SettingsViewModel>(viewResult.Model);

        Assert.Equal(10, model.ItemsPerPage);
        Assert.True(model.EnableNotifications);
    }

    [Fact]
    public async Task Index_Get_ReturnsViewWithExistingSettings()
    {
        var context = GetInMemoryDbContext();

        // Seed settings
        context.Settings.Add(new Setting { Key = "ItemsPerPage", Value = "25" });
        context.Settings.Add(new Setting { Key = "EnableNotifications", Value = "false" });
        await context.SaveChangesAsync();

        var controller = new SettingsController(context);

        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<SettingsViewModel>(viewResult.Model);

        Assert.Equal(25, model.ItemsPerPage);
        Assert.False(model.EnableNotifications);
    }

    [Fact]
    public async Task Index_Post_InvalidModel_ReturnsViewWithModel()
    {
        var context = GetInMemoryDbContext();

        var controller = new SettingsController(context);
        controller.ModelState.AddModelError("ItemsPerPage", "Required");

        var model = new SettingsViewModel();

        var result = await controller.Index(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
    }

    [Fact]
    public async Task Index_Post_ValidModel_SavesSettingsAndRedirects()
    {
        var context = GetInMemoryDbContext();
        var controller = new SettingsController(context);
        SetupTempData(controller); // Това е важно!

        var model = new SettingsViewModel
        {
            ItemsPerPage = 50,
            EnableNotifications = false
        };

        var result = await controller.Index(model);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(SettingsController.Index), redirectResult.ActionName);

        Assert.True(controller.TempData.ContainsKey("SuccessMessage"));

        var itemsPerPageSetting = await context.Settings.FirstOrDefaultAsync(s => s.Key == "ItemsPerPage");
        var enableNotificationsSetting = await context.Settings.FirstOrDefaultAsync(s => s.Key == "EnableNotifications");

        Assert.NotNull(itemsPerPageSetting);
        Assert.Equal("50", itemsPerPageSetting.Value);

        Assert.NotNull(enableNotificationsSetting);
        Assert.Equal("False", enableNotificationsSetting.Value);
    }
}
