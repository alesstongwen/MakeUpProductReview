using System;
using System.Collections.Generic;

namespace MakeupReviewApp.Models.ViewModels
{
    public class UserProfile
    {
        public User User { get; set; }
        public string ProfilePicture { get; set; }
        public DateTime JoinDate { get; set; }
        public List<Review> Reviews { get; set; } = new List<Review>();
        public List<Product> Wishlist { get; set; } = new List<Product>();
    }
}