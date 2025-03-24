using System.Linq;
using MakeupReviewApp.Repositories;
using MakeupReviewApp.Models;

namespace MakeupReviewApp.Services
{
    public class WishlistService
    {
        private readonly MockUserRepository _userRepo;
        private readonly MockProductRepository _productRepo;

        public WishlistService(MockUserRepository userRepo, MockProductRepository productRepo)
        {
            _userRepo = userRepo;
            _productRepo = productRepo;
        }

        public bool AddToWishlist(string userEmail, int productId)
        {
            var userProfile = _userRepo.GetUserProfileByEmail(userEmail);
            if (userProfile == null)
            {
                return false;
            }

            var product = _productRepo.GetProductById(productId);
            if (product == null)
            {
                return false;
            }

            if (!userProfile.Wishlist.Any(p => p.Id == productId))
            {
                userProfile.Wishlist.Add(product);
            }

            return true;
        }

        public bool RemoveFromWishlist(string userEmail, int productId)
        {
            var userProfile = _userRepo.GetUserProfileByEmail(userEmail);
            if (userProfile == null)
            {
                return false;
            }

            var product = userProfile.Wishlist.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                userProfile.Wishlist.Remove(product);
            }

            return true;
        }
    }
}