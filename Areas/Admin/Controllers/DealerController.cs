using AutoShop.Data;
using AutoShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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

        // GET: Admin/Dealer/All
        public async Task<IActionResult> All()
        {
            var dealers = await _context.Dealers.ToListAsync();
            return View(dealers);
        }

        // GET: Admin/Dealer/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Dealer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Dealer dealer)
        {
            if (!ModelState.IsValid)
            {
                // Временно: Покажи грешките в ModelState в конзолата за дебъгване
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Поле: {kv.Key}, Грешка: {error.ErrorMessage}");
                    }
                }

                return View(dealer);
            }

            _context.Add(dealer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(All));
        }

        // GET: Admin/Dealer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var dealer = await _context.Dealers.FindAsync(id);
            if (dealer == null)
                return NotFound();

            return View(dealer);
        }

        // POST: Admin/Dealer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Dealer dealer)
        {
            if (id != dealer.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(dealer);

            try
            {
                _context.Update(dealer);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DealerExists(dealer.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(All));
        }

        // GET: Admin/Dealer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var dealer = await _context.Dealers
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dealer == null)
                return NotFound();

            return View(dealer);
        }

        // POST: Admin/Dealer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dealer = await _context.Dealers.FindAsync(id);

            if (dealer != null)
            {
                _context.Dealers.Remove(dealer);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(All));
        }

        private bool DealerExists(int id)
        {
            return _context.Dealers.Any(e => e.Id == id);
        }
    }
}
