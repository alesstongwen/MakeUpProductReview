using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MakeupReviewApp.Models;
using MakeupReviewApp.Repositories;

namespace MakeupReviewApp.Services
{
    public class WishlistService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MockProductRepository _productRepo;

        public WishlistService(UserManager<ApplicationUser> userManager, MockProductRepository productRepo)
        {
            _userManager = userManager;
            _productRepo = productRepo;
        }

        public async Task<bool> AddToWishlistAsync(string userEmail, int productId)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
                return false;

            var product = _productRepo.GetProductById(productId);
            if (product == null)
                return false;

            if (user.Wishlist == null)
                user.Wishlist = new List<Product>();

            if (!user.Wishlist.Any(p => p.Id == productId))
            {
                user.Wishlist.Add(product);
                await _userManager.UpdateAsync(user); // Persist the change
            }

            return true;
        }

        public async Task<bool> RemoveFromWishlistAsync(string userEmail, int productId)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null || user.Wishlist == null)
                return false;

            var product = user.Wishlist.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                user.Wishlist.Remove(product);
                await _userManager.UpdateAsync(user); // Persist the change
            }

            return true;
        }
    }
}
