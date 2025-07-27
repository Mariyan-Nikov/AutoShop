using AutoShop.Areas.Admin.Controllers;
using AutoShop.Models;
using AutoShop.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class DealerControllerTests
{
    private DealerController GetControllerWithMocks(
        Mock<IDealerService> dealerServiceMock,
        IHeaderDictionary? headers = null)
    {
        var controller = new DealerController(dealerServiceMock.Object);

        // Setup TempData - важно за TempData използване
        controller.TempData = new TempDataDictionary(
            new DefaultHttpContext(), Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

        // Setup HttpContext and Headers
        var httpContext = new DefaultHttpContext();
        if (headers != null)
        {
            httpContext.Request.Headers.Clear();
            foreach (var h in headers)
            {
                httpContext.Request.Headers[h.Key] = h.Value;
            }
        }
        controller.ControllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        // Setup Url Helper (за да не хвърля NullReferenceException при Url.Action)
        var urlHelperMock = new Mock<IUrlHelper>();
        urlHelperMock.Setup(x => x.Action(It.IsAny<UrlActionContext>()))
            .Returns("/Admin/Dealer/All");
        controller.Url = urlHelperMock.Object;

        return controller;
    }

    [Fact]
    public async Task All_ReturnsViewWithDealers()
    {
        var dealerList = new List<Dealer>()
        {
            new Dealer { Id = 1, Name = "Dealer1" },
            new Dealer { Id = 2, Name = "Dealer2" }
        };

        var dealerServiceMock = new Mock<IDealerService>();
        dealerServiceMock.Setup(s => s.GetAllDealersAsync())
            .ReturnsAsync(dealerList);

        var controller = GetControllerWithMocks(dealerServiceMock);

        var result = await controller.All();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Dealer>>(viewResult.Model);
        Assert.Equal(2, model.Count());
    }

    [Fact]
    public async Task Create_Post_InvalidModel_NonAjax_ReturnsViewWithModel()
    {
        var dealerServiceMock = new Mock<IDealerService>();

        var controller = GetControllerWithMocks(dealerServiceMock);
        controller.ModelState.AddModelError("Name", "Required");

        var dealer = new Dealer();

        var result = await controller.Create(dealer);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(dealer, viewResult.Model);
    }

    [Fact]
    public async Task Create_Post_ValidModel_NonAjax_AddsDealerAndRedirects()
    {
        var dealerServiceMock = new Mock<IDealerService>();
        dealerServiceMock.Setup(s => s.AddDealerAsync(It.IsAny<Dealer>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var controller = GetControllerWithMocks(dealerServiceMock);

        var dealer = new Dealer { Name = "Dealer1" };

        var result = await controller.Create(dealer);

        dealerServiceMock.Verify(s => s.AddDealerAsync(dealer), Times.Once);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("All", redirectResult.ActionName);

        Assert.True(controller.TempData.ContainsKey("SuccessMessage"));
    }

    [Fact]
    public async Task Create_Post_InvalidModel_Ajax_ReturnsBadRequest()
    {
        var dealerServiceMock = new Mock<IDealerService>();

        var headers = new HeaderDictionary
        {
            { "X-Requested-With", "XMLHttpRequest" }
        };
        var controller = GetControllerWithMocks(dealerServiceMock, headers);

        controller.ModelState.AddModelError("Name", "Required");

        var dealer = new Dealer();

        var result = await controller.Create(dealer);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

        var errors = Assert.IsAssignableFrom<IEnumerable<object>>(badRequestResult.Value);
        Assert.NotEmpty(errors);
    }

    [Fact]
    public async Task Delete_ExistingDealer_ReturnsSuccessJson()
    {
        var dealer = new Dealer { Id = 1, Name = "Dealer1" };

        var dealerServiceMock = new Mock<IDealerService>();
        dealerServiceMock.Setup(s => s.GetDealerByIdAsync(1))
            .ReturnsAsync(dealer);
        dealerServiceMock.Setup(s => s.DeleteDealerAsync(1))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var controller = GetControllerWithMocks(dealerServiceMock);

        var result = await controller.Delete(1);

        dealerServiceMock.Verify(s => s.DeleteDealerAsync(1), Times.Once);

        var jsonResult = Assert.IsType<JsonResult>(result);
        dynamic value = jsonResult.Value!;
        Assert.True(value.success);
        Assert.Contains("изтрит", value.message.ToString());
    }

    [Fact]
    public async Task Delete_NonExistingDealer_ReturnsFailureJson()
    {
        var dealerServiceMock = new Mock<IDealerService>();
        dealerServiceMock.Setup(s => s.GetDealerByIdAsync(1))
            .ReturnsAsync((Dealer?)null);

        var controller = GetControllerWithMocks(dealerServiceMock);

        var result = await controller.Delete(1);

        var jsonResult = Assert.IsType<JsonResult>(result);
        dynamic value = jsonResult.Value!;
        Assert.False(value.success);
        Assert.Contains("не беше намерен", value.message.ToString());
    }

    [Fact]
    public async Task Edit_Post_InvalidModel_ReturnsBadRequest()
    {
        var dealerServiceMock = new Mock<IDealerService>();

        var controller = GetControllerWithMocks(dealerServiceMock);
        controller.ModelState.AddModelError("Name", "Required");

        var dealer = new Dealer();

        var result = await controller.Edit(dealer);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

        var errors = Assert.IsAssignableFrom<IEnumerable<object>>(badRequestResult.Value);
        Assert.NotEmpty(errors);
    }

    [Fact]
    public async Task Edit_Post_NonExistingDealer_ReturnsNotFound()
    {
        var dealerServiceMock = new Mock<IDealerService>();
        dealerServiceMock.Setup(s => s.GetDealerByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Dealer?)null);

        var controller = GetControllerWithMocks(dealerServiceMock);

        var dealer = new Dealer { Id = 1 };

        var result = await controller.Edit(dealer);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        dynamic value = notFoundResult.Value!;
        Assert.Contains("не съществува", value.message.ToString());
    }

    [Fact]
    public async Task Edit_Post_ValidModel_UpdatesAndReturnsOkJson()
    {
        var existingDealer = new Dealer { Id = 1, Name = "OldName", Address = "OldAddress", PhoneNumber = "OldPhone", Email = "old@example.com" };

        var dealerServiceMock = new Mock<IDealerService>();
        dealerServiceMock.Setup(s => s.GetDealerByIdAsync(1))
            .ReturnsAsync(existingDealer);
        dealerServiceMock.Setup(s => s.UpdateDealerAsync(It.IsAny<Dealer>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var controller = GetControllerWithMocks(dealerServiceMock);

        var updatedDealer = new Dealer
        {
            Id = 1,
            Name = "NewName",
            Address = "NewAddress",
            PhoneNumber = "NewPhone",
            Email = "new@example.com"
        };

        var result = await controller.Edit(updatedDealer);

        dealerServiceMock.Verify(s => s.UpdateDealerAsync(It.Is<Dealer>(d =>
            d.Id == updatedDealer.Id &&
            d.Name == updatedDealer.Name &&
            d.Address == updatedDealer.Address &&
            d.PhoneNumber == updatedDealer.PhoneNumber &&
            d.Email == updatedDealer.Email
        )), Times.Once);

        var okResult = Assert.IsType<OkObjectResult>(result);
        dynamic value = okResult.Value!;
        Assert.NotNull(value.redirectUrl);
        Assert.Equal("/Admin/Dealer/All", value.redirectUrl);
    }
}
