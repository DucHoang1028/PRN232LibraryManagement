using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IBookService _bookService;
        private readonly IMemberService _memberService;

        public ReviewsController(
            IReviewService reviewService,
            IBookService bookService,
            IMemberService memberService)
        {
            _reviewService = reviewService;
            _bookService = bookService;
            _memberService = memberService;
        }

        [HttpGet("list/book/{bookId}")]
        public async Task<IActionResult> GetBookReviews(Guid bookId)
        {
            var book = _bookService.GetBookById(bookId);
            if (book == null)
            {
                return NotFound(new { message = "Book not found" });
            }

            var reviews = await _reviewService.GetBookReviewsAsync(bookId);
            var averageRating = await _reviewService.GetBookAverageRatingAsync(bookId);

            return Ok(new 
            { 
                reviews, 
                averageRating,
                ratingCount = reviews.Count
            });
        }

        [HttpGet("details/{reviewId}")]
        public async Task<IActionResult> GetReview(Guid reviewId)
        {
            var review = await _reviewService.GetReviewAsync(reviewId);
            if (review == null)
            {
                return NotFound(new { message = "Review not found" });
            }

            return Ok(review);
        }

        [HttpGet("findBy/member/{memberId}/book/{bookId}")]
        public async Task<IActionResult> GetMemberBookReview(Guid memberId, Guid bookId)
        {
            var review = await _reviewService.GetMemberBookReviewAsync(memberId, bookId);
            if (review == null)
            {
                return NotFound(new { message = "Review not found" });
            }

            return Ok(review);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateReview([FromBody] Review review)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = string.Join("; ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(new { message = $"Invalid model: {errors}" });
                }

                // If user is authenticated, try to get their member ID
                if (User.Identity?.IsAuthenticated == true)
                {
                    var userEmail = User.FindFirstValue(ClaimTypes.Email);
                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        var member = _memberService.GetMemberByEmail(userEmail);
                        if (member != null)
                        {
                            review.MemberId = member.MemberId;
                        }
                    }
                }

                // Use provided MemberId or a default guest ID if not authenticated
                if (review.MemberId == Guid.Empty)
                {
                    // Use default guest ID
                    review.MemberId = Guid.Parse("00000000-0000-0000-0000-000000000099");
                }

                // Check if the book exists
                var book = _bookService.GetBookById(review.BookId);
                if (book == null)
                {
                    return BadRequest(new { message = $"Book not found with ID: {review.BookId}" });
                }

                // Check if the user already reviewed this book (if authenticated)
                if (User.Identity?.IsAuthenticated == true)
                {
                    var existingReview = await _reviewService.GetMemberBookReviewAsync(review.MemberId, review.BookId);
                    if (existingReview != null)
                    {
                        return BadRequest(new { message = "You have already reviewed this book" });
                    }
                }

                var newReview = await _reviewService.AddReviewAsync(review);
                return CreatedAtAction(nameof(GetReview), new { reviewId = newReview.ReviewId }, newReview);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpPut("update/{reviewId}")]
        public async Task<IActionResult> UpdateReview(Guid reviewId, [FromBody] Review updatedReview)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var review = await _reviewService.GetReviewAsync(reviewId);
            if (review == null)
            {
                return NotFound(new { message = "Review not found" });
            }

            // Get current user
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var member = _memberService.GetMemberByEmail(userEmail);
            if (member == null || review.MemberId != member.MemberId)
            {
                return Forbid();
            }

            // Update only allowed fields
            review.Rating = updatedReview.Rating;
            review.Comment = updatedReview.Comment;

            await _reviewService.UpdateReviewAsync(review);
            return Ok(review);
        }

        [Authorize]
        [HttpDelete("delete/{reviewId}")]
        public async Task<IActionResult> DeleteReview(Guid reviewId)
        {
            var review = await _reviewService.GetReviewAsync(reviewId);
            if (review == null)
            {
                return NotFound(new { message = "Review not found" });
            }

            // Get current user
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var member = _memberService.GetMemberByEmail(userEmail);
            
            // Allow only review author or staff to delete
            if (member == null || (review.MemberId != member.MemberId && member.Role != "Staff"))
            {
                return Forbid();
            }

            await _reviewService.DeleteReviewAsync(reviewId);
            return NoContent();
        }
    }
} 