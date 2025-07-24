using AutoShop.Data;
using AutoShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AutoShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DealerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DealerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> All()
        {
            var dealers = await _context.Dealers.ToListAsync();
            return View(dealers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Dealer dealer)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"Error: {error.ErrorMessage}");
                    }
                    return View(dealer);
                }

                _context.Add(dealer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Успешно добавихте дилър {dealer.Name}";
                return RedirectToAction(nameof(All));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Грешка при създаване: {ex.Message}";
                return View(dealer);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var dealer = await _context.Dealers.FindAsync(id);
            if (dealer == null) return NotFound();

            return View(dealer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Dealer dealer)
        {
            if (id != dealer.Id) return NotFound();

            try
            {
                if (!ModelState.IsValid) return View(dealer);

                _context.Update(dealer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Успешно обновихте {dealer.Name}";
                return RedirectToAction(nameof(All));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Грешка при редакция: {ex.Message}";
                return View(dealer);
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var dealer = await _context.Dealers.FirstOrDefaultAsync(d => d.Id == id);
            if (dealer == null) return NotFound();

            return View(dealer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var dealer = await _context.Dealers.FindAsync(id);
                if (dealer == null) return NotFound();

                _context.Dealers.Remove(dealer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Успешно изтрихте {dealer.Name}";
                return RedirectToAction(nameof(All));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Грешка при изтриване: {ex.Message}";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
}