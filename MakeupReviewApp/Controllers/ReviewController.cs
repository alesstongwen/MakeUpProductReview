using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MakeupReviewApp.Services;
using MakeupReviewApp.Models;

namespace MakeupReviewApp.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly ReviewService _reviewService;

        public ReviewController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [Authorize]
        public IActionResult Create(int productId)
        {
            return View(new Review { ProductId = productId });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(Review review)
        {
            if (ModelState.IsValid)
            {
                _reviewService.AddReview(review);
                return RedirectToAction("Details", "Product", new { id = review.ProductId });
            }
            return View(review);
        }
    }
}