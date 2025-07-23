using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessObjects
{
    public class ReviewDAO
    {
        private readonly ApplicationDbContext _context;

        public ReviewDAO(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<Review>> GetBookReviewsAsync(Guid bookId)
        {
            return await _context.Reviews
                .Include(r => r.Member)
                .Where(r => r.BookId == bookId)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }
        
        public async Task<double> GetBookAverageRatingAsync(Guid bookId)
        {
            var ratings = await _context.Reviews
                .Where(r => r.BookId == bookId)
                .Select(r => r.Rating)
                .ToListAsync();
                
            return ratings.Any() ? ratings.Average() : 0;
        }
        
        public async Task<Review?> GetReviewAsync(Guid reviewId)
        {
            return await _context.Reviews
                .Include(r => r.Member)
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.ReviewId == reviewId);
        }
        
        public async Task<Review?> GetMemberBookReviewAsync(Guid memberId, Guid bookId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.MemberId == memberId && r.BookId == bookId);
        }
        
        public async Task<Review> AddReviewAsync(Review review)
        {
            review.ReviewId = Guid.NewGuid();
            review.CreatedDate = DateTime.UtcNow;
            
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            
            return review;
        }
        
        public async Task<Review> UpdateReviewAsync(Review review)
        {
            review.UpdatedDate = DateTime.UtcNow;
            
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
            
            return review;
        }
        
        public async Task DeleteReviewAsync(Guid reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }
    }
} 