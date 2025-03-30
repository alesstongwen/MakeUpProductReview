using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MakeupReviewApp.Models.ViewModels;
using MakeupReviewApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MakeupReviewApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            _logger.LogInformation("Login page accessed");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            _logger.LogInformation($"Login attempt for email: {model.Email}");

            // Detailed model validation logging
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _logger.LogWarning($"Model Validation Error: {error.ErrorMessage}");
                    }
                }
                return View(model);
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    _logger.LogWarning($"Login failed: No user found with email {model.Email}");
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    isPersistent: false,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {model.Email} logged in successfully");
                    return RedirectToAction("Index", "Home");
                }

                // Detailed failure logging
                if (result.IsLockedOut)
                {
                    _logger.LogWarning($"User {model.Email} account is locked out");
                    ModelState.AddModelError(string.Empty, "Account locked. Please contact support.");
                }
                else if (result.IsNotAllowed)
                {
                    _logger.LogWarning($"Login not allowed for {model.Email}");
                    ModelState.AddModelError(string.Empty, "Login not allowed.");
                }
                else
                {
                    _logger.LogWarning($"Invalid login attempt for {model.Email}");
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error during login for {model.Email}");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            _logger.LogInformation("Register page accessed");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            _logger.LogInformation($"Registration attempt for email: {model.Email}");

            // Detailed model validation logging
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _logger.LogWarning($"Model Validation Error: {error.ErrorMessage}");
                    }
                }
                return View(model);
            }

            try
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName, // Optional: only if your RegisterViewModel includes this
                    JoinDate = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {model.Email} registered successfully");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                // Log specific registration errors
                foreach (var error in result.Errors)
                {
                    _logger.LogWarning($"Registration error for {model.Email}: {error.Code} - {error.Description}");
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error during registration for {model.Email}");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred during registration.");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Login", "Account");
        }
    }
}