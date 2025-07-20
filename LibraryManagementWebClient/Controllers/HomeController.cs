using System.Diagnostics;
using LibraryManagementWebClient.Models;
using LibraryManagementWebClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementWebClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILibraryApiService _apiService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILibraryApiService apiService, ILogger<HomeController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // If user is Staff, show staff dashboard
                if (User.IsInRole("Staff"))
                {
                    var dashboardViewModel = new DashboardViewModel
                    {
                        TotalBooks = 0,
                        TotalMembers = 0,
                        ActiveLoans = 0,
                        OverdueLoans = 0,
                        RecentBooks = new List<Book>(),
                        RecentLoans = new List<Loan>()
                    };

                    // Get statistics
                    var books = await _apiService.GetBooksAsync();
                    var members = await _apiService.GetMembersAsync();
                    var activeLoans = await _apiService.GetActiveLoansAsync();
                    var overdueLoans = await _apiService.GetOverdueLoansAsync();

                    dashboardViewModel.TotalBooks = books.Count;
                    dashboardViewModel.TotalMembers = members.Count;
                    dashboardViewModel.ActiveLoans = activeLoans.Count;
                    dashboardViewModel.OverdueLoans = overdueLoans.Count;
                    dashboardViewModel.RecentBooks = books.Take(5).ToList();
                    dashboardViewModel.RecentLoans = activeLoans.Take(5).ToList();

                    return View("StaffDashboard", dashboardViewModel);
                }
                else
                {
                    // For guests and members, show public library interface
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

                    return View("PublicLibrary", viewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page");
                TempData["Error"] = "Error loading library data. Please try again.";
                
                if (User.IsInRole("Staff"))
                {
                    return View("StaffDashboard", new DashboardViewModel());
                }
                else
                {
                    return View("PublicLibrary", new CustomerDashboardViewModel());
                }
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
