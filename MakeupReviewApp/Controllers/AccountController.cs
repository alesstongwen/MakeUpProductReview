using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using MakeupReviewApp.Models.ViewModels;
using MakeupReviewApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MakeupReviewApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            AppDbContext context,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
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
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email
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
    }
}