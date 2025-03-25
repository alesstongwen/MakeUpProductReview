using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using MakeupReviewApp.Models;
using MakeupReviewApp.Repositories;
using Microsoft.AspNetCore.Identity;

namespace MakeupReviewApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User loginUser)
        {
            if (loginUser == null || string.IsNullOrEmpty(loginUser.Email) || string.IsNullOrEmpty(loginUser.Password))
            {
                ModelState.AddModelError("", "Email and Password are required.");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User newUser)
        {
            if (string.IsNullOrEmpty(newUser.Email) || string.IsNullOrEmpty(newUser.Password) || string.IsNullOrEmpty(newUser.FullName))
            {
                ModelState.AddModelError("", "All fields are required.");
                return View();
            }

            var user = new IdentityUser { UserName = newUser.Email, Email = newUser.Email };
            var result = await _userManager.CreateAsync(user, newUser.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View();
        }
    }
}