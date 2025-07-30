using AutoShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;  // Замени с твоя namespace за ErrorViewModel

public class ErrorController : Controller
{
    [Route("Error/404")]
    public IActionResult NotFoundError()
    {
        var model = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };
        return View("NotFound", model);
    }

    [Route("Error/403")]
    public IActionResult ForbiddenError()
    {
        var model = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };
        return View("Forbidden", model);
    }

    [Route("Error/500")]
    public IActionResult ServerError()
    {
        var model = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };
        return View("Error", model);
    }
}
