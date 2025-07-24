using AutoShop.Models;
using AutoShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
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
            try
            {
                var categories = await _categoryService.GetAllAsync();
                return View(categories);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Грешка при зареждане на категориите: {ex.Message}";
                return View(Array.Empty<Category>());
            }
        }

        // GET: /Admin/Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(category);
                }

                await _categoryService.AddAsync(category);
                TempData["SuccessMessage"] = $"Успешно създадохте категория '{category.Name}'";
                return RedirectToAction(nameof(All));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Грешка при създаване: {ex.Message}";
                return View(category);
            }
        }

        // GET: /Admin/Category/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var category = await _categoryService.GetByIdAsync(id);
                if (category == null)
                {
                    TempData["ErrorMessage"] = "Категорията не беше намерена";
                    return RedirectToAction(nameof(All));
                }
                return View(category);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Грешка при зареждане: {ex.Message}";
                return RedirectToAction(nameof(All));
            }
        }

        // POST: /Admin/Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            try
            {
                if (id != category.Id)
                {
                    TempData["ErrorMessage"] = "Невалиден идентификатор на категория";
                    return RedirectToAction(nameof(All));
                }

                if (!ModelState.IsValid)
                {
                    return View(category);
                }

                await _categoryService.UpdateAsync(category);
                TempData["SuccessMessage"] = $"Успешно обновихте '{category.Name}'";
                return RedirectToAction(nameof(All));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Грешка при редакция: {ex.Message}";
                return View(category);
            }
        }

        // GET: /Admin/Category/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var category = await _categoryService.GetByIdAsync(id);
                if (category == null)
                {
                    TempData["ErrorMessage"] = "Категорията не беше намерена";
                    return RedirectToAction(nameof(All));
                }
                return View(category);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Грешка при зареждане: {ex.Message}";
                return RedirectToAction(nameof(All));
            }
        }

        // POST: /Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var category = await _categoryService.GetByIdAsync(id);
                if (category == null)
                {
                    TempData["ErrorMessage"] = "Категорията не беше намерена";
                    return RedirectToAction(nameof(All));
                }

                await _categoryService.DeleteAsync(id);
                TempData["SuccessMessage"] = $"Успешно изтрихте '{category.Name}'";
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