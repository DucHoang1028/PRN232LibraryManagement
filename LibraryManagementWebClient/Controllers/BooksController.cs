using LibraryManagementWebClient.Models;
using LibraryManagementWebClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementWebClient.Controllers
{
    public class BooksController : Controller
    {
        private readonly ILibraryApiService _apiService;

        public BooksController(ILibraryApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(string? searchTerm)
        {
            try
            {
                List<Book> books;
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    books = await _apiService.SearchBooksAsync(title: searchTerm);
                }
                else
                {
                    books = await _apiService.GetBooksAsync();
                }
                return View(books);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading books: {ex.Message}";
                return View(new List<Book>());
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var book = await _apiService.GetBookAsync(id);
                if (book == null)
                {
                    return NotFound();
                }

                // Check if book has active loans
                bool hasActiveLoans = await _apiService.HasActiveLoansAsync(id);
                ViewBag.HasActiveLoans = hasActiveLoans;
                
                return View(book);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading book details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                var publishers = await _apiService.GetPublishersAsync();
                ViewBag.Publishers = publishers;
                return View(new Book());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading publishers: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _apiService.CreateBookAsync(book);
                    TempData["Success"] = "Book created successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating book: {ex.Message}");
            }

            var publishers = await _apiService.GetPublishersAsync();
            ViewBag.Publishers = publishers;
            return View(book);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var book = await _apiService.GetBookAsync(id);
                if (book == null)
                {
                    return NotFound();
                }
                
                // Check if book has active loans
                bool hasActiveLoans = await _apiService.HasActiveLoansAsync(id);
                if (hasActiveLoans)
                {
                    TempData["Error"] = "Cannot edit this book because it currently has active loans.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                var publishers = await _apiService.GetPublishersAsync();
                ViewBag.Publishers = publishers;
                return View(book);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading book: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Book book)
        {
            try
            {
                // Double-check if book has active loans before proceeding
                bool hasActiveLoans = await _apiService.HasActiveLoansAsync(id);
                if (hasActiveLoans)
                {
                    TempData["Error"] = "Cannot edit this book because it currently has active loans.";
                    return RedirectToAction(nameof(Details), new { id });
                }
                
                if (ModelState.IsValid)
                {
                    await _apiService.UpdateBookAsync(id, book);
                    TempData["Success"] = "Book updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                // Check if error is related to active loans
                if (ex.Message.Contains("active loans"))
                {
                    TempData["Error"] = "Cannot edit this book because it currently has active loans.";
                    return RedirectToAction(nameof(Details), new { id });
                }
                
                ModelState.AddModelError("", $"Error updating book: {ex.Message}");
            }

            var publishers = await _apiService.GetPublishersAsync();
            ViewBag.Publishers = publishers;
            return View(book);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var book = await _apiService.GetBookAsync(id);
                if (book == null)
                {
                    return NotFound();
                }
                
                // Check if book has active loans
                bool hasActiveLoans = await _apiService.HasActiveLoansAsync(id);
                if (hasActiveLoans)
                {
                    TempData["Error"] = "Cannot delete this book because it currently has active loans.";
                    return RedirectToAction(nameof(Details), new { id });
                }
                
                return View(book);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading book: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                // Double-check if book has active loans before proceeding
                bool hasActiveLoans = await _apiService.HasActiveLoansAsync(id);
                if (hasActiveLoans)
                {
                    TempData["Error"] = "Cannot delete this book because it currently has active loans.";
                    return RedirectToAction(nameof(Details), new { id });
                }
                
                var success = await _apiService.DeleteBookAsync(id);
                if (success)
                {
                    TempData["Success"] = "Book deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to delete book.";
                }
            }
            catch (Exception ex)
            {
                // Check if error is related to active loans
                if (ex.Message.Contains("active loans"))
                {
                    TempData["Error"] = "Cannot delete this book because it currently has active loans.";
                    return RedirectToAction(nameof(Details), new { id = id });
                }
                
                TempData["Error"] = $"Error deleting book: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Search(string? title, string? author, string? category, DateTime? publicationDate)
        {
            try
            {
                var books = await _apiService.SearchBooksAsync(title, author, category, publicationDate);
                return View("Index", books);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error searching books: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 