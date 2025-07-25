using AutoShop.Models;
using AutoShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AutoShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: /Admin/Category/All
        public async Task<IActionResult> All()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }

        // GET: /Admin/Category/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            await _categoryService.AddAsync(category);
            TempData["SuccessMessage"] = "Категорията е създадена успешно!";
            return RedirectToAction(nameof(All));
        }

        // DELETE: /Admin/Category/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                return Json(new { success = false, message = "Категорията не беше намерена." });
            }

            await _categoryService.DeleteAsync(id);
            return Json(new { success = true, message = $"Категорията '{category.Name}' беше изтрита." });
        }


        //Еdit

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound("Категорията не е намерена.");
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category model)
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

            var existing = await _categoryService.GetByIdAsync(model.Id);
            if (existing == null)
            {
                return NotFound(new { message = "Категорията не съществува." });
            }

            existing.Name = model.Name;
            existing.Description = model.Description;

            await _categoryService.UpdateAsync(existing);

            return Ok(new { message = "Категорията е обновена успешно." });
        }
    }
}

