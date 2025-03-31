using Microsoft.AspNetCore.Identity;
using MakeupReviewApp.Models;

namespace MakeupReviewApp.Services
{
    public class CustomPasswordValidator : IPasswordValidator<ApplicationUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user, string password)
        {
            var errors = new List<IdentityError>();

            // 🔒 Custom rule: must include at least one special character
            if (!password.Any(char.IsSymbol) && !password.Any(char.IsPunctuation))
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordRequiresSpecialCharacter",
                    Description = "Password must contain at least one special character."
                });
            }

            // 🔒 Custom rule: must include at least one lowercase letter
            if (!password.Any(char.IsLower))
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordRequiresLower",
                    Description = "Password must contain at least one lowercase letter."
                });
            }

            return Task.FromResult(errors.Any()
                ? IdentityResult.Failed(errors.ToArray())
                : IdentityResult.Success);
        }
    }
}
