using BusinessObjects;
using DataAccessObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ReviewDAO _reviewDAO;

        public ReviewRepository(ApplicationDbContext context)
        {
            _reviewDAO = new ReviewDAO(context);
        }

        public async Task<List<Review>> GetBookReviewsAsync(Guid bookId)
        {
            return await _reviewDAO.GetBookReviewsAsync(bookId);
        }

        public async Task<double> GetBookAverageRatingAsync(Guid bookId)
        {
            return await _reviewDAO.GetBookAverageRatingAsync(bookId);
        }

        public async Task<Review?> GetReviewAsync(Guid reviewId)
        {
            return await _reviewDAO.GetReviewAsync(reviewId);
        }

        public async Task<Review?> GetMemberBookReviewAsync(Guid memberId, Guid bookId)
        {
            return await _reviewDAO.GetMemberBookReviewAsync(memberId, bookId);
        }

        public async Task<Review> AddReviewAsync(Review review)
        {
            return await _reviewDAO.AddReviewAsync(review);
        }

        public async Task<Review> UpdateReviewAsync(Review review)
        {
            return await _reviewDAO.UpdateReviewAsync(review);
        }

        public async Task DeleteReviewAsync(Guid reviewId)
        {
            await _reviewDAO.DeleteReviewAsync(reviewId);
        }
    }
} 