using MakeupReviewApp.Repositories;
using MakeupReviewApp.Models;

namespace MakeupReviewApp.Services
{
    public class ReviewService
    {
        private readonly MockReviewRepository _reviewRepo;

        public ReviewService(MockReviewRepository reviewRepo)
        {
            _reviewRepo = reviewRepo;
        }

        public void AddReview(Review review)
        {
            _reviewRepo.AddReview(review);
        }
    }
}