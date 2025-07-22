using AutoShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    // GET: /Review
    public async Task<IActionResult> Index()
    {
        var reviews = await _reviewService.GetAllReviewsAsync();
        return View(reviews);
    }

    // GET: /Review/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var review = await _reviewService.GetReviewByIdAsync(id);
        if (review == null)
        {
            return NotFound();
        }

        return View(review);
    }

    // GET: /Review/Create?carId=5
    public IActionResult Create(int carId)
    {
        var review = new Review { CarId = carId };
        ViewBag.Cars = new SelectList(_reviewService.GetAllCars(), "Id", "Brand", carId);
        return View(review);
    }

    // POST: /Review/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Review review)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Cars = new SelectList(_reviewService.GetAllCars(), "Id", "Brand", review.CarId);
            return View(review);
        }

        review.CreatedOn = DateTime.UtcNow;

        await _reviewService.AddReviewAsync(review);
        return RedirectToAction(nameof(Index));
    }

    // GET: /Review/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var review = await _reviewService.GetReviewByIdAsync(id);
        if (review == null)
        {
            return NotFound();
        }
        ViewBag.Cars = new SelectList(_reviewService.GetAllCars(), "Id", "Brand", review.CarId);
        return View(review);
    }

    // POST: /Review/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Review review)
    {
        if (id != review.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Cars = new SelectList(_reviewService.GetAllCars(), "Id", "Brand", review.CarId);
            return View(review);
        }

        await _reviewService.UpdateReviewAsync(review);
        return RedirectToAction(nameof(Index));
    }

    // GET: /Review/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var review = await _reviewService.GetReviewByIdAsync(id);
        if (review == null)
        {
            return NotFound();
        }

        return View(review);
    }

    // POST: /Review/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _reviewService.DeleteReviewAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
