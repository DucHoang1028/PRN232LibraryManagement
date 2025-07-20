using LibraryManagementWebClient.Models;
using LibraryManagementWebClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementWebClient.Controllers
{
    public class LoanHistoryController : Controller
    {
        private readonly ILibraryApiService _apiService;
        private readonly ILogger<LoanHistoryController> _logger;

        public LoanHistoryController(ILibraryApiService apiService, ILogger<LoanHistoryController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var memberId = GetCurrentMemberId();
                if (memberId == Guid.Empty)
                {
                    TempData["Error"] = "Member information not found. Please contact support.";
                    return RedirectToAction("Index", "Customer");
                }

                var member = await _apiService.GetMemberAsync(memberId);
                var loans = await _apiService.GetMemberLoansAsync(memberId);
                var fines = await _apiService.GetMemberFinesAsync(memberId);
                var canCheckout = await _apiService.CanMemberCheckoutAsync(memberId);
                var activeLoanCount = await _apiService.GetMemberActiveLoanCountAsync(memberId);

                var viewModel = new LoanHistoryViewModel
                {
                    Member = member,
                    Loans = loans,
                    Fines = fines,
                    CanCheckout = canCheckout,
                    ActiveLoanCount = activeLoanCount,
                    TotalLoans = loans.Count,
                    ActiveLoans = loans.Where(l => l.ReturnDate == null).ToList(),
                    OverdueLoans = loans.Where(l => l.DueDate < DateTime.Today && l.ReturnDate == null).ToList(),
                    TotalFines = fines.Sum(f => f.Amount),
                    UnpaidFines = fines.Where(f => !f.IsPaid).Sum(f => f.Amount)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading loan history");
                TempData["Error"] = "Error loading your loan history. Please try again.";
                return RedirectToAction("Index", "Customer");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnBook(Guid loanId)
        {
            try
            {
                var success = await _apiService.ReturnBookAsync(loanId);
                if (success)
                {
                    TempData["Success"] = "Book returned successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to return book. Please try again.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error returning book");
                TempData["Error"] = "Error returning book. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RenewBook(Guid loanId)
        {
            try
            {
                var success = await _apiService.RenewBookAsync(loanId);
                if (success)
                {
                    TempData["Success"] = "Book renewed successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to renew book. Please try again.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error renewing book");
                TempData["Error"] = "Error renewing book. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }

        private Guid GetCurrentMemberId()
        {
            var memberIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (memberIdClaim != null && Guid.TryParse(memberIdClaim.Value, out Guid memberId))
            {
                return memberId;
            }
            return Guid.Empty;
        }
    }
} 