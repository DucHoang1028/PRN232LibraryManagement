using LibraryManagementWebClient.Models;
using LibraryManagementWebClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementWebClient.Controllers
{
    public class PublishersController : Controller
    {
        private readonly ILibraryApiService _apiService;

        public PublishersController(ILibraryApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var publishers = await _apiService.GetPublishersAsync();
                return View(publishers);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading publishers: {ex.Message}";
                return View(new List<Publisher>());
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var publisher = await _apiService.GetPublisherAsync(id);
                if (publisher == null)
                {
                    return NotFound();
                }
                return View(publisher);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading publisher details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Create()
        {
            return View(new Publisher());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Publisher publisher)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _apiService.CreatePublisherAsync(publisher);
                    TempData["Success"] = "Publisher created successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating publisher: {ex.Message}");
            }

            return View(publisher);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var publisher = await _apiService.GetPublisherAsync(id);
                if (publisher == null)
                {
                    return NotFound();
                }
                return View(publisher);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading publisher: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Publisher publisher)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _apiService.UpdatePublisherAsync(id, publisher);
                    TempData["Success"] = "Publisher updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating publisher: {ex.Message}");
            }

            return View(publisher);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var publisher = await _apiService.GetPublisherAsync(id);
                if (publisher == null)
                {
                    return NotFound();
                }
                return View(publisher);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading publisher: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var success = await _apiService.DeletePublisherAsync(id);
                if (success)
                {
                    TempData["Success"] = "Publisher deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to delete publisher.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting publisher: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
} 