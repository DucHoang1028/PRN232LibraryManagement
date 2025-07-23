using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<List<Review>> GetBookReviewsAsync(Guid bookId)
        {
            return await _reviewRepository.GetBookReviewsAsync(bookId);
        }

        public async Task<double> GetBookAverageRatingAsync(Guid bookId)
        {
            return await _reviewRepository.GetBookAverageRatingAsync(bookId);
        }

        public async Task<Review?> GetReviewAsync(Guid reviewId)
        {
            return await _reviewRepository.GetReviewAsync(reviewId);
        }

        public async Task<Review?> GetMemberBookReviewAsync(Guid memberId, Guid bookId)
        {
            return await _reviewRepository.GetMemberBookReviewAsync(memberId, bookId);
        }

        public async Task<Review> AddReviewAsync(Review review)
        {
            return await _reviewRepository.AddReviewAsync(review);
        }

        public async Task<Review> UpdateReviewAsync(Review review)
        {
            return await _reviewRepository.UpdateReviewAsync(review);
        }

        public async Task DeleteReviewAsync(Guid reviewId)
        {
            await _reviewRepository.DeleteReviewAsync(reviewId);
        }
    }
} 