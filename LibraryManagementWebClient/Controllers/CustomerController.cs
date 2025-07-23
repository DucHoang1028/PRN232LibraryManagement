using LibraryManagementWebClient.Models;
using LibraryManagementWebClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace LibraryManagementWebClient.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ILibraryApiService _apiService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ILibraryApiService apiService, ILogger<CustomerController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var books = await _apiService.GetBooksAsync();
                var featuredBooks = books.Where(b => b.IsActive && b.AvailableCopies > 0)
                                       .OrderByDescending(b => b.CreatedDate)
                                       .Take(6)
                                       .ToList();

                var viewModel = new CustomerDashboardViewModel
                {
                    FeaturedBooks = featuredBooks,
                    TotalBooks = books.Count,
                    AvailableBooks = books.Count(b => b.AvailableCopies > 0),
                    RecentBooks = books.Where(b => b.IsActive)
                                     .OrderByDescending(b => b.CreatedDate)
                                     .Take(8)
                                     .ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customer dashboard");
                TempData["Error"] = "Error loading library data. Please try again.";
                return View(new CustomerDashboardViewModel());
            }
        }

        public async Task<IActionResult> Catalog(string searchTerm, string category, string publisher)
        {
            try
            {
                var books = await _apiService.GetBooksAsync();
                var publishers = await _apiService.GetPublishersAsync();

                // Apply filters
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    books = books.Where(b => 
                        b.Title.ToLower().Contains(searchTerm.ToLower()) ||
                        b.Isbn.ToLower().Contains(searchTerm.ToLower()) ||
                        b.Description?.ToLower().Contains(searchTerm.ToLower()) == true
                    ).ToList();
                }

                if (!string.IsNullOrEmpty(publisher))
                {
                    books = books.Where(b => b.PublisherId.ToString() == publisher).ToList();
                }

                var viewModel = new CatalogViewModel
                {
                    Books = books.Where(b => b.IsActive).ToList(),
                    Publishers = publishers.Where(p => p.IsActive).ToList(),
                    SearchTerm = searchTerm,
                    SelectedPublisher = publisher
                };

                ViewBag.Publishers = publishers.Where(p => p.IsActive).ToList();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading catalog");
                TempData["Error"] = "Error loading catalog. Please try again.";
                
                // Set ViewBag.Publishers to empty list to prevent null reference
                ViewBag.Publishers = new List<Publisher>();
                
                return View(new CatalogViewModel
                {
                    Books = new List<Book>(),
                    Publishers = new List<Publisher>(),
                    SearchTerm = searchTerm,
                    SelectedPublisher = publisher
                });
            }
        }

        public async Task<IActionResult> BookDetails(Guid id)
        {
            try
            {
                var book = await _apiService.GetBookByIdAsync(id);
                if (book == null)
                {
                    return NotFound();
                }

                // Get reviews for the book
                var (reviews, averageRating, ratingCount) = await _apiService.GetBookReviewsAsync(id);
                ViewBag.Reviews = reviews;
                ViewBag.AverageRating = averageRating;
                ViewBag.RatingCount = ratingCount;

                // If user is logged in, get their review if it exists
                if (User.Identity?.IsAuthenticated == true && User.IsInRole("Member"))
                {
                    var memberId = GetCurrentMemberId();
                    var userReview = await _apiService.GetMemberBookReviewAsync(memberId, id);
                    ViewBag.UserReview = userReview;
                    
                    // Check if the member has unreturned or overdue books
                    ViewBag.HasUnreturnedBooks = await _apiService.HasUnreturnedBooksAsync(memberId);
                    ViewBag.HasOverdueBooks = await _apiService.HasOverdueBooksAsync(memberId);
                    
                    // Check if user already has this specific book checked out
                    ViewBag.HasThisBookCheckedOut = await _apiService.HasBookCheckedOutAsync(memberId, id);
                }

                return View(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading book details");
                TempData["Error"] = "Error loading book details. Please try again.";
                return RedirectToAction(nameof(Catalog));
            }
        }

        public async Task<IActionResult> Search(string q)
        {
            try
            {
                if (string.IsNullOrEmpty(q))
                {
                    return RedirectToAction(nameof(Catalog));
                }

                var books = await _apiService.GetBooksAsync();
                var searchResults = books.Where(b => 
                    b.IsActive && (
                        b.Title.ToLower().Contains(q.ToLower()) ||
                        b.Isbn.ToLower().Contains(q.ToLower()) ||
                        b.Description?.ToLower().Contains(q.ToLower()) == true ||
                        b.Publisher?.Name.ToLower().Contains(q.ToLower()) == true
                    )
                ).ToList();

                var viewModel = new SearchResultsViewModel
                {
                    SearchTerm = q,
                    Results = searchResults,
                    TotalResults = searchResults.Count
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing search");
                TempData["Error"] = "Error performing search. Please try again.";
                return RedirectToAction(nameof(Catalog));
            }
        }

        public async Task<IActionResult> About()
        {
            try
            {
                var books = await _apiService.GetBooksAsync();
                var members = await _apiService.GetMembersAsync();
                var publishers = await _apiService.GetPublishersAsync();

                var viewModel = new AboutViewModel
                {
                    TotalBooks = books.Count,
                    TotalMembers = members.Count,
                    TotalPublishers = publishers.Count,
                    AvailableBooks = books.Count(b => b.AvailableCopies > 0)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading about page");
                return View(new AboutViewModel());
            }
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> CheckoutBook(Guid bookId, DateTime startDate, DateTime endDate)
        {
            try
            {
                if (!User.IsInRole("Member"))
                {
                    return RedirectToAction("AccessDenied", "Auth");
                }

                // Validate date range
                if (startDate < DateTime.Today)
                {
                    TempData["Error"] = "Start date cannot be in the past.";
                    return RedirectToAction("BookDetails", new { id = bookId });
                }

                if (endDate <= startDate)
                {
                    TempData["Error"] = "End date must be after start date.";
                    return RedirectToAction("BookDetails", new { id = bookId });
                }

                var loanPeriod = (endDate - startDate).Days;
                if (loanPeriod > 14)
                {
                    TempData["Error"] = "Loan period cannot exceed 14 days.";
                    return RedirectToAction("BookDetails", new { id = bookId });
                }

                // Get the current user's member ID
                var memberId = GetCurrentMemberId();
                if (memberId == Guid.Empty)
                {
                    TempData["Error"] = "Member information not found. Please contact support.";
                    return RedirectToAction("BookDetails", new { id = bookId });
                }

                // Get the member by email to check for additional validation
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var member = await _apiService.GetMemberByEmailAsync(userEmail);
                    if (member != null)
                    {
                        // Check if the user has overdue books
                        if (await _apiService.HasOverdueBooksAsync(member.MemberId))
                        {
                            TempData["Error"] = "You have overdue books. Please return them before checking out new books.";
                            return RedirectToAction("BookDetails", new { id = bookId });
                        }
                        
                        // Check if the user already has this book checked out
                        if (await _apiService.HasBookCheckedOutAsync(member.MemberId, bookId))
                        {
                            TempData["Error"] = "You already have this book checked out.";
                            return RedirectToAction("BookDetails", new { id = bookId });
                        }
                    }
                }

                // Create loan using the API
                var loan = await _apiService.CheckoutBookAsync(bookId, memberId);
                
                TempData["Success"] = $"Book '{loan.Book?.Title}' has been checked out successfully! Return by {endDate:MMMM dd, yyyy}.";
                return RedirectToAction("BookDetails", new { id = bookId });
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("400"))
            {
                // Extract the error message from the response if possible
                string errorMessage = "Unable to check out book. ";
                
                // Try to extract a more detailed message from the inner exception
                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                {
                    var errorContent = ex.InnerException.Message;
                    if (errorContent.Contains("Member is not eligible"))
                        errorMessage = "You have reached the maximum number of books you can check out.";
                    else if (errorContent.Contains("Book is not available"))
                        errorMessage = "This book is no longer available for checkout.";
                    else if (errorContent.Contains("already have this book"))
                        errorMessage = "You already have this book checked out.";
                    else if (errorContent.Contains("overdue books"))
                        errorMessage = "You have overdue books. Please return them before checking out new books.";
                    else
                        errorMessage = "Error: " + errorContent;
                }
                
                _logger.LogError(ex, errorMessage);
                TempData["Error"] = errorMessage;
                return RedirectToAction("BookDetails", new { id = bookId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking out book");
                
                // Provide a more user-friendly message based on the error message
                string errorMessage = "Error checking out book.";
                
                if (ex.Message.Contains("overdue"))
                {
                    errorMessage = "You have overdue books. Please return them before checking out new books.";
                }
                else if (ex.Message.Contains("not eligible"))
                {
                    errorMessage = "You are not eligible to check out more books at this time. You may have reached the maximum limit.";
                }
                else if (ex.Message.Contains("not available"))
                {
                    errorMessage = "This book is not available for checkout. It may be on loan or reserved.";
                }
                else if (ex.Message.Contains("already have this book"))
                {
                    errorMessage = "You already have this book checked out.";
                }
                else
                {
                    // Include part of the actual error for debugging
                    errorMessage = $"Error checking out book: {ex.Message.Split('.').FirstOrDefault()}";
                }
                
                TempData["Error"] = errorMessage;
                return RedirectToAction("BookDetails", new { id = bookId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(Review review)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please provide a valid rating";
                return RedirectToAction("BookDetails", new { id = review.BookId });
            }

            try
            {
                // Try to get member ID if user is authenticated
                if (User.Identity?.IsAuthenticated == true)
                {
                    var memberId = GetCurrentMemberId();
                    review.MemberId = memberId;
                    
                    // Check if user already reviewed this book
                    var existingReview = await _apiService.GetMemberBookReviewAsync(memberId, review.BookId);
                    if (existingReview != null)
                    {
                        TempData["Error"] = "You have already reviewed this book.";
                        return RedirectToAction("BookDetails", new { id = review.BookId });
                    }
                }

                // Create the review
                await _apiService.CreateReviewAsync(review);
                TempData["Success"] = "Your review was submitted successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding review");
                TempData["Error"] = "Error adding review. Please try again.";
            }

            return RedirectToAction("BookDetails", new { id = review.BookId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> UpdateReview(Review review)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid review data. Please try again.";
                return RedirectToAction("BookDetails", new { id = review.BookId });
            }

            try
            {
                var memberId = GetCurrentMemberId();
                
                // Check if this review belongs to the current user
                var existingReview = await _apiService.GetReviewAsync(review.ReviewId);
                if (existingReview == null || existingReview.MemberId != memberId)
                {
                    TempData["Error"] = "You are not authorized to edit this review.";
                    return RedirectToAction("BookDetails", new { id = review.BookId });
                }

                // Update only the rating and comment
                existingReview.Rating = review.Rating;
                existingReview.Comment = review.Comment;

                await _apiService.UpdateReviewAsync(review.ReviewId, existingReview);
                TempData["Success"] = "Your review was updated successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review");
                TempData["Error"] = "Error updating review. Please try again.";
            }

            return RedirectToAction("BookDetails", new { id = review.BookId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member,Staff")]
        public async Task<IActionResult> DeleteReview(Guid reviewId)
        {
            try
            {
                var review = await _apiService.GetReviewAsync(reviewId);
                if (review == null)
                {
                    TempData["Error"] = "Review not found.";
                    return RedirectToAction("Index");
                }

                var bookId = review.BookId;
                var memberId = GetCurrentMemberId();
                
                // Only allow staff or the review owner to delete
                if (!User.IsInRole("Staff") && review.MemberId != memberId)
                {
                    TempData["Error"] = "You are not authorized to delete this review.";
                    return RedirectToAction("BookDetails", new { id = bookId });
                }

                await _apiService.DeleteReviewAsync(reviewId);
                TempData["Success"] = "Review deleted successfully!";
                return RedirectToAction("BookDetails", new { id = bookId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review");
                TempData["Error"] = "Error deleting review. Please try again.";
                return RedirectToAction("Index");
            }
        }

        private Guid GetCurrentMemberId()
        {
            try
            {
                // Try various claim types that might contain the member ID
                var memberIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                    ?? User.FindFirst("MemberId") 
                    ?? User.FindFirst("Id");

                if (memberIdClaim != null && Guid.TryParse(memberIdClaim.Value, out Guid memberId))
                {
                    return memberId;
                }

                // If we can't get it from claims, try to get it by email
                var emailClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Email);
                if (emailClaim != null)
                {
                    var member = _apiService.GetMemberByEmailAsync(emailClaim.Value).Result;
                    if (member != null)
                    {
                        return member.MemberId;
                    }
                }
                
                // Log all claims for debugging
                var claims = string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}"));
                _logger.LogWarning($"Could not find member ID in claims: {claims}");
                
                // Fallback to default member ID if not found
                return Guid.Parse("00000000-0000-0000-0000-000000000002");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current member ID");
                return Guid.Parse("00000000-0000-0000-0000-000000000002"); // Default fallback
            }
        }
    }
} 