using AutoShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;

    // Конструктор за инжектиране на сервиза за ревюта
    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    // GET: /Review
    // Показва списък с всички ревюта
    public async Task<IActionResult> Index()
    {
        var reviews = await _reviewService.GetAllReviewsAsync();
        return View(reviews);
    }

    // GET: /Review/Details/5
    // Показва детайли за конкретно ревю по ID
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
    // Показва форма за създаване на ново ревю, с предварително зададен CarId
    public IActionResult Create(int carId)
    {
        var review = new Review { CarId = carId };
        // Създава dropdown списък с коли, за избор (предварително избрана кола)
        ViewBag.Cars = new SelectList(_reviewService.GetAllCars(), "Id", "Brand", carId);
        return View(review);
    }

    // POST: /Review/Create
    // Обработва изпратената форма за ново ревю
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Review review)
    {
        // Ако моделът не е валиден, връща формата с грешките
        if (!ModelState.IsValid)
        {
            ViewBag.Cars = new SelectList(_reviewService.GetAllCars(), "Id", "Brand", review.CarId);
            return View(review);
        }

        // Задава дата и час на създаване (UTC)
        review.CreatedOn = DateTime.UtcNow;

        // Добавя ревюто чрез сервиза
        await _reviewService.AddReviewAsync(review);
        return RedirectToAction(nameof(Index));
    }

    // GET: /Review/Edit/5
    // Показва форма за редакция на съществуващо ревю
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
    // Обработва редакцията на ревю
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Review review)
    {
        // Проверява дали ID-тата съвпадат
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
    // Показва страница за потвърждение на изтриване
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
    // Изтрива ревюто след потвърждение
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _reviewService.DeleteReviewAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
