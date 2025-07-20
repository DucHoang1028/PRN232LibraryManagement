using LibraryManagementWebClient.Models;
using LibraryManagementWebClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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

                // Get the current user's member ID (in real app, get from user claims)
                var memberId = GetCurrentMemberId();
                if (memberId == Guid.Empty)
                {
                    TempData["Error"] = "Member information not found. Please contact support.";
                    return RedirectToAction("BookDetails", new { id = bookId });
                }

                // Create loan using the API
                var loan = await _apiService.CheckoutBookAsync(bookId, memberId);
                
                TempData["Success"] = $"Book '{loan.Book?.Title}' has been checked out successfully! Return by {endDate:MMMM dd, yyyy}.";
                return RedirectToAction("BookDetails", new { id = bookId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking out book");
                TempData["Error"] = "Error checking out book. Please try again.";
                return RedirectToAction("BookDetails", new { id = bookId });
            }
        }

        private Guid GetCurrentMemberId()
        {
            // Get member ID from user claims
            var memberIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (memberIdClaim != null && Guid.TryParse(memberIdClaim.Value, out Guid memberId))
            {
                return memberId;
            }
            
            // Fallback to default member ID if not found
            return Guid.Parse("00000000-0000-0000-0000-000000000002");
        }
    }
} 