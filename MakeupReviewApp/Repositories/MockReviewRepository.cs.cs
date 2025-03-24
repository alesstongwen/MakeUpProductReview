using System;
using System.Collections.Generic;
using System.Linq;
using MakeupReviewApp.Models;

namespace MakeupReviewApp.Repositories
{
    public class MockReviewRepository
    {
        private static List<Review> _reviews = new List<Review>
        {
            new Review { Id = 1, ProductId = 1, UserName = "Alice", Rating = 5, Comment = "Amazing lipstick! Long-lasting and vibrant.", Date = DateTime.Now.AddDays(-3) },
            new Review { Id = 2, ProductId = 1, UserName = "Brenda", Rating = 4, Comment = "Good quality, but a bit dry for my lips.", Date = DateTime.Now.AddDays(-2) },
            new Review { Id = 3, ProductId = 2, UserName = "Carol", Rating = 5, Comment = "Best foundation I've ever used!", Date = DateTime.Now.AddDays(-1) }
        };

        public List<Review> GetAllReviews()
        {
            return _reviews;
        }

        public List<Review> GetReviewsByUser(string userName)
        {
            return _reviews.Where(r => r.UserName == userName).ToList();
        }

        public List<Review> GetReviewsByProductId(int productId)
        {
            return _reviews.Where(r => r.ProductId == productId).ToList();
        }

        public void AddReview(Review review)
        {
            review.Id = _reviews.Count + 1;
            review.Date = DateTime.Now;
            _reviews.Add(review);
            Console.WriteLine($"Review added: {review.Comment} for ProductId {review.ProductId}");
        }
    }
}
