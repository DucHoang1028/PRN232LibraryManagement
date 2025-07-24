using LibraryManagementWebClient.Models;
using LibraryManagementWebClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementWebClient.Controllers
{
    public class MembersController : Controller
    {
        private readonly ILibraryApiService _apiService;

        public MembersController(ILibraryApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var members = await _apiService.GetMembersAsync();
                return View(members);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading members: {ex.Message}";
                return View(new List<Member>());
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var member = await _apiService.GetMemberAsync(id);
                if (member == null)
                {
                    return NotFound();
                }
                return View(member);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading member details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Create()
        {
            return View(new Member { JoinDate = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Member member)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    member.IsActive = true;
                    member.CreatedDate = DateTime.UtcNow;
                    await _apiService.CreateMemberAsync(member);
                    TempData["Success"] = "Member created successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating member: {ex.Message}");
            }

            return View(member);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var member = await _apiService.GetMemberAsync(id);
                if (member == null)
                {
                    return NotFound();
                }
                return View(member);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading member: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Member member)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _apiService.UpdateMemberAsync(id, member);
                    TempData["Success"] = "Member updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating member: {ex.Message}");
            }

            return View(member);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var member = await _apiService.GetMemberAsync(id);
                if (member == null)
                {
                    return NotFound();
                }
                return View(member);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading member: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var success = await _apiService.DeleteMemberAsync(id);
                if (success)
                {
                    TempData["Success"] = "Member deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to delete member.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting member: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }


    }
} 