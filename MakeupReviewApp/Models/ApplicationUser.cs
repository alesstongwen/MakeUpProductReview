using Microsoft.AspNetCore.Identity;

namespace MakeupReviewApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Add any custom fields you want
        public string? FullName { get; set; }
        public DateTime JoinDate { get; set; } = DateTime.Now;
        public virtual ICollection<Product> Wishlist { get; set; } = new List<Product>();
    }
}
