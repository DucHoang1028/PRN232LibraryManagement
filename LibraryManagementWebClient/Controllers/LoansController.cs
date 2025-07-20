using LibraryManagementWebClient.Models;
using LibraryManagementWebClient.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementWebClient.Controllers
{
    public class LoansController : Controller
    {
        private readonly ILibraryApiService _apiService;

        public LoansController(ILibraryApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var loans = await _apiService.GetLoansAsync();
                return View(loans);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading loans: {ex.Message}";
                return View(new List<Loan>());
            }
        }

        public async Task<IActionResult> Active()
        {
            try
            {
                var loans = await _apiService.GetActiveLoansAsync();
                return View("Index", loans);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading active loans: {ex.Message}";
                return View("Index", new List<Loan>());
            }
        }

        public async Task<IActionResult> Overdue()
        {
            try
            {
                var loans = await _apiService.GetOverdueLoansAsync();
                return View("Index", loans);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading overdue loans: {ex.Message}";
                return View("Index", new List<Loan>());
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var loan = await _apiService.GetLoanAsync(id);
                if (loan == null)
                {
                    return NotFound();
                }
                return View(loan);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading loan details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Checkout()
        {
            try
            {
                var books = await _apiService.GetBooksAsync();
                var members = await _apiService.GetMembersAsync();
                
                ViewBag.Books = books.Where(b => b.AvailableCopies > 0 && b.IsActive).ToList();
                ViewBag.Members = members.Where(m => m.IsActive).ToList();
                
                return View(new CheckoutViewModel());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading checkout data: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loan = await _apiService.CheckoutBookAsync(model.BookId, model.MemberId);
                    TempData["Success"] = "Book checked out successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error checking out book: {ex.Message}");
            }

            var books = await _apiService.GetBooksAsync();
            var members = await _apiService.GetMembersAsync();
            
            ViewBag.Books = books.Where(b => b.AvailableCopies > 0 && b.IsActive).ToList();
            ViewBag.Members = members.Where(m => m.IsActive).ToList();
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(Guid id)
        {
            try
            {
                var success = await _apiService.ReturnBookAsync(id);
                if (success)
                {
                    TempData["Success"] = "Book returned successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to return book.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error returning book: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Renew(Guid id)
        {
            try
            {
                var success = await _apiService.RenewBookAsync(id);
                if (success)
                {
                    TempData["Success"] = "Book renewed successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to renew book.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error renewing book: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var loan = await _apiService.GetLoanAsync(id);
                if (loan == null)
                {
                    return NotFound();
                }
                return View(loan);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading loan: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Loan loan)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _apiService.UpdateLoanAsync(id, loan);
                    TempData["Success"] = "Loan updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating loan: {ex.Message}");
            }

            return View(loan);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var loan = await _apiService.GetLoanAsync(id);
                if (loan == null)
                {
                    return NotFound();
                }
                return View(loan);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading loan: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var success = await _apiService.DeleteLoanAsync(id);
                if (success)
                {
                    TempData["Success"] = "Loan deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to delete loan.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting loan: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }

    public class CheckoutViewModel
    {
        [Required]
        public Guid BookId { get; set; }
        
        [Required]
        public Guid MemberId { get; set; }
    }
} 