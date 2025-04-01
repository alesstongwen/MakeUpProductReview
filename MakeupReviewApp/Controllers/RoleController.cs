using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MakeupReviewApp.Models.ViewModels;
using MakeupReviewApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace MakeupReviewApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(IdentityRole role)
        {
            var existingRole = await _roleManager.FindByIdAsync(role.Id);
            if (existingRole == null) return NotFound();

            existingRole.Name = role.Name;
            var result = await _roleManager.UpdateAsync(existingRole);

            if (result.Succeeded)
                return RedirectToAction("Index");

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(role);
        }
        [HttpGet]
        public async Task<IActionResult> ManageUsers(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return NotFound();

            var users = _userManager.Users.ToList();
            var viewModel = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                viewModel.Add(new UserRoleViewModel
                {
                    UserId = user.Id,
                    IsSelected = await _userManager.IsInRoleAsync(user, role.Name)
                });
            }

            ViewBag.RoleId = roleId;
            ViewBag.RoleName = role.Name;
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> ManageUsers(List<UserRoleViewModel> model, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return NotFound();

            foreach (var user in model)
            {
                var appUser = await _userManager.FindByIdAsync(user.UserId);

                if (user.IsSelected && !(await _userManager.IsInRoleAsync(appUser, role.Name)))
                {
                    await _userManager.AddToRoleAsync(appUser, role.Name);
                }
                else if (!user.IsSelected && await _userManager.IsInRoleAsync(appUser, role.Name))
                {
                    await _userManager.RemoveFromRoleAsync(appUser, role.Name);
                }
            }

            return RedirectToAction("Index");
        }

    }
}
