using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MakeupReviewApp.Models;
using MakeupReviewApp.Models.ViewModels;
using MakeupReviewApp.Repositories;

namespace MakeupReviewApp.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MockReviewRepository _reviewRepo;
        private readonly MockProductRepository _productRepo;

        public UserProfileController(
            UserManager<ApplicationUser> userManager,
            MockReviewRepository reviewRepo,
            MockProductRepository productRepo)
        {
            _userManager = userManager;
            _reviewRepo = reviewRepo;
            _productRepo = productRepo;
        }

        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Manually build the UserProfile ViewModel
            var reviews = _reviewRepo.GetAllReviews().Where(r => r.UserName == user.FullName).ToList();

            var userProfile = new UserProfile
            {
                User = user,
                ProfilePicture = null, // You can replace this with an actual path if implemented
                JoinDate = user.JoinDate,
                Reviews = reviews,
                Wishlist = new List<Product>() // Optional: If you store Wishlist in database, load it here
            };

            return View(userProfile);
        }
    }
}
