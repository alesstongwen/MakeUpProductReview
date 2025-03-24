using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MakeupReviewApp.Services;

namespace MakeupReviewApp.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly WishlistService _wishlistService;

        public WishlistController(WishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpPost]
        public IActionResult AddToWishlist(int productId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var result = _wishlistService.AddToWishlist(userEmail, productId);
            if (!result)
            {
                return NotFound("User profile or product not found.");
            }

            return RedirectToAction("Profile", "UserProfile");
        }

        [HttpPost]
        public IActionResult RemoveFromWishlist(int productId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var result = _wishlistService.RemoveFromWishlist(userEmail, productId);
            if (!result)
            {
                return NotFound("User profile not found.");
            }

            return RedirectToAction("Profile", "UserProfile");
        }
    }
}