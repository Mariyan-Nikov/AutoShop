using AutoShop.Data;
using AutoShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq; // Added for any LINQ operations if needed
using System.Threading.Tasks;

using AutoShop.Services.Interfaces; // Important: Add this using

namespace AutoShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DealerController : Controller
    {
        private readonly IDealerService _dealerService; // Injecting the service interface

        public DealerController(IDealerService dealerService)
        {
            _dealerService = dealerService;
        }

        /// <summary>
        /// Displays a list of all dealers.
        /// </summary>
        public async Task<IActionResult> All()
        {
            var dealers = await _dealerService.GetAllDealersAsync();
            return View(dealers);
        }

        /// <summary>
        /// Displays the form for creating a new dealer.
        /// </summary>
        [HttpGet] // Explicitly marking as GET request
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Handles the POST request for creating a new dealer.
        /// Uses standard HTTP POST (not AJAX).
        /// </summary>
        /// <param name="dealer">The data for the new dealer submitted from the form.</param>
        /// <returns>Returns the View with validation errors if invalid, or redirects to the dealer list on success.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Dealer dealer)
        {
            // Checks if the submitted data is valid according to the Dealer model's data annotations.
            if (!ModelState.IsValid)
            {
                // If the model is not valid, returns the same view (Create.cshtml).
                // Validation messages will be automatically displayed on the page.
                return View(dealer);
            }

            // If validation is successful, adds the dealer via the service.
            await _dealerService.AddDealerAsync(dealer);

            // Sets a success message in TempData, which can be displayed after redirection.
            TempData["SuccessMessage"] = $"Дилър '{dealer.Name}' е добавен успешно!";

            // Redirects the browser to the page showing all dealers.
            // This causes a full page reload.
            return RedirectToAction(nameof(All));
        }

        /// <summary>
        /// Displays the form for editing an existing dealer.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dealer = await _dealerService.GetDealerByIdAsync(id);
            if (dealer == null)
            {
                return NotFound("Дилърът не е намерен.");
            }
            return View(dealer);
        }

        /// <summary>
        /// Handles the POST request for editing a dealer. (Designed for AJAX submission)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Dealer dealer)
        {
            // Check if IDs match for security
            if (id != dealer.Id)
            {
                ModelState.AddModelError("", "Несъответствие в ID на дилъра.");
            }

            if (!ModelState.IsValid)
            {
                // Collect all validation errors in JSON format
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key, // Key is the field name (e.g., "Name", "Email")
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                // Return 400 Bad Request with the errors
                return BadRequest(new { success = false, errors = errors });
            }

            try
            {
                var existingDealer = await _dealerService.GetDealerByIdAsync(id);
                if (existingDealer == null)
                {
                    // Return 404 Not Found with a JSON message
                    return NotFound(new { success = false, message = "Дилърът не съществува." });
                }

                // Update the properties of the existing object
                existingDealer.Name = dealer.Name;
                existingDealer.Address = dealer.Address;
                existingDealer.PhoneNumber = dealer.PhoneNumber;
                existingDealer.Email = dealer.Email;

                await _dealerService.UpdateDealerAsync(existingDealer);

                // Return 200 OK with a JSON success message
                return Ok(new { success = true, message = $"Дилър '{existingDealer.Name}' е обновен успешно." });
            }
            catch (Exception ex)
            {
                // Return 500 Internal Server Error with a JSON error message
                return StatusCode(500, new { success = false, message = $"Възникна грешка при обновяване на дилъра: {ex.Message}" });
            }
        }

        /// <summary>
        /// Displays the confirmation page for deleting a dealer.
        /// </summary>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var dealer = await _dealerService.GetDealerByIdAsync(id.Value);
            if (dealer == null) return NotFound();

            return View(dealer);
        }

        /// <summary>
        /// Handles the POST request for confirmed dealer deletion. (Designed for AJAX submission)
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var dealer = await _dealerService.GetDealerByIdAsync(id);
                if (dealer == null)
                {
                    // Return JSON response if dealer not found
                    return Json(new { success = false, message = "Дилърът не беше намерен за изтриване." });
                }

                await _dealerService.DeleteDealerAsync(id);
                // Return JSON response for success
                return Json(new { success = true, message = $"Успешно изтрихте {dealer.Name}" });
            }
            catch (Exception ex)
            {
                // Return JSON response for error
                return StatusCode(500, Json(new { success = false, message = $"Грешка при изтриване на дилър: {ex.Message}" }));
            }
        }
    }
}
