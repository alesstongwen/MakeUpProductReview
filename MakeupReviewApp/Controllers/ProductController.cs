using Microsoft.AspNetCore.Mvc;
using MakeupReviewApp.Repositories;
using MakeupReviewApp.Models;

public class ProductController : Controller
{
    private readonly MockProductRepository _productRepo = new MockProductRepository();
    private readonly MockReviewRepository _reviewRepo = new MockReviewRepository();

    public IActionResult Index()
    {
        var products = _productRepo.GetAllProducts();
        return View(products);
    }

    public IActionResult Details(int id)
    {
        var product = _productRepo.GetProductById(id);
        if (product == null) return NotFound();

        var reviews = _reviewRepo.GetReviewsByProductId(id);
        ViewBag.Reviews = reviews;

        Console.WriteLine($"[DEBUG] Loading Product {id} - Found {reviews.Count} reviews");

        return View(product);
    }

}