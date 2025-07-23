using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetBookReviewsAsync(Guid bookId);
        Task<double> GetBookAverageRatingAsync(Guid bookId);
        Task<Review?> GetReviewAsync(Guid reviewId);
        Task<Review?> GetMemberBookReviewAsync(Guid memberId, Guid bookId);
        Task<Review> AddReviewAsync(Review review);
        Task<Review> UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(Guid reviewId);
    }
} 