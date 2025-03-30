using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MakeupReviewApp.Models;
using MakeupReviewApp.Models.ViewModels;

namespace MakeupReviewApp.Services
{
    public class UserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserProfile?> GetUserProfileAsync(string email, List<Review> reviews)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;

            return new UserProfile
            {
                User = user,
                ProfilePicture = "/images/default-profile.png", // or user.ProfilePicture if added
                JoinDate = user.JoinDate,
                Reviews = reviews
            };
        }

        public async Task<bool> AddUserAsync(ApplicationUser newUser, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(newUser.Email);
            if (existingUser != null) return false;

            var result = await _userManager.CreateAsync(newUser, password);
            return result.Succeeded;
        }

        public async Task<ApplicationUser?> ValidateUserAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                return user;
            }
            return null;
        }
    }
}
