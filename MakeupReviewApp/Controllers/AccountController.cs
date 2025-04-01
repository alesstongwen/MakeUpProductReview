using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MakeupReviewApp.Models.ViewModels;
using MakeupReviewApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


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

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (remoteError != null)
            {
                _logger.LogError("Remote error: {RemoteError}", remoteError);
                ViewBag.ErrorTitle = "External login error";
                ViewBag.ErrorMessage = $"Error from external provider: {remoteError}";
                return View("Error");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                _logger.LogError("External login info is null");
                ViewBag.ErrorTitle = "External login error";
                ViewBag.ErrorMessage = "Could not get external login information.";
                return View("Error");
            }

            var email = info.Principal?.FindFirstValue(ClaimTypes.Email);
            _logger.LogInformation("✅ External login email received: {Email}", email);

            // Try logging in with external login
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            // Try finding the user by email
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                // Check if login already associated
                var logins = await _userManager.GetLoginsAsync(user);
                if (!logins.Any(l => l.LoginProvider == info.LoginProvider && l.ProviderKey == info.ProviderKey))
                {
                    var addLoginResult = await _userManager.AddLoginAsync(user, info);
                    if (!addLoginResult.Succeeded)
                    {
                        foreach (var error in addLoginResult.Errors)
                        {
                            _logger.LogError("AddLoginAsync error: {Code} - {Description}", error.Code, error.Description);
                        }
                        ViewBag.ErrorTitle = "External login failed";
                        ViewBag.ErrorMessage = "We couldn’t link your external account. Please try logging in manually.";
                        return View("Error");
                    }
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }

            // No user exists yet → create one
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = email?.Split('@')[0],
                JoinDate = DateTime.Now
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    _logger.LogError("Create user error: {Code} - {Description}", error.Code, error.Description);
                }

                ViewBag.ErrorTitle = "User creation failed";
                ViewBag.ErrorMessage = "We couldn’t create your user account.";
                return View("Error");
            }

            var linkResult = await _userManager.AddLoginAsync(user, info);
            if (!linkResult.Succeeded)
            {
                foreach (var error in linkResult.Errors)
                {
                    _logger.LogError("AddLoginAsync error: {Code} - {Description}", error.Code, error.Description);
                }

                ViewBag.ErrorTitle = "External login failed after account creation";
                ViewBag.ErrorMessage = "We couldn’t link your external account. Please try logging in manually.";
                return View("Error");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(returnUrl);
        }
    }
}