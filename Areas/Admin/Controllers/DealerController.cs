using AutoShop.Models;
using AutoShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace AutoShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DealerController : Controller
    {
        private readonly IDealerService _dealerService;

        public DealerController(IDealerService dealerService)
        {
            _dealerService = dealerService;
        }

        // GET: /Admin/Dealer/All
        public async Task<IActionResult> All()
        {
            var dealers = await _dealerService.GetAllDealersAsync();
            return View(dealers);
        }

        // GET: /Admin/Dealer/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/Dealer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Dealer dealer)
        {
            if (!ModelState.IsValid)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new
                        {
                            Key = x.Key,
                            Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        });

                    return BadRequest(errors);
                }

                return View(dealer);
            }

            await _dealerService.AddDealerAsync(dealer);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, message = "Дилърът е създаден успешно!" });
            }

            TempData["SuccessMessage"] = "Дилърът е създаден успешно!";
            return RedirectToAction(nameof(All));
        }
        // POST: /Admin/Dealer/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var dealer = await _dealerService.GetDealerByIdAsync(id);
            if (dealer == null)
            {
                return Json(new { success = false, message = "Дилърът не беше намерен." });
            }

            await _dealerService.DeleteDealerAsync(id);
            return Json(new { success = true, message = $"Дилърът '{dealer.Name}' беше изтрит." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Dealer model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new
                    {
                        Key = x.Key,
                        Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    });

                return BadRequest(errors);
            }

            var existing = await _dealerService.GetDealerByIdAsync(model.Id);
            if (existing == null)
            {
                return NotFound(new { message = "Дилърът не съществува." });
            }

            existing.Name = model.Name;
            existing.Address = model.Address;
            existing.PhoneNumber = model.PhoneNumber;
            existing.Email = model.Email;

            await _dealerService.UpdateDealerAsync(existing);

            // Връщаме JSON с команда за пренасочване
            return Ok(new { redirectUrl = Url.Action("All", "Dealer", new { area = "Admin" }) });
        }
    }
}
