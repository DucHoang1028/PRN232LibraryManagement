using System.Threading.Tasks;
using LibraryManagementWebClient.Services;
using LibraryManagementWebClient.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementWebClient.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IApiService _apiService;
        public AdminController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(string? roleFilter = null)
        {
            var selectedRole = string.IsNullOrEmpty(roleFilter) ? "member" : roleFilter;

            var users = await _apiService.GetAllUsersAsync();
            var models = users.Select(u => new UserSummaryViewModel
            {
                Id = u.MemberId,
                FullName = u.FirstName + " " + u.LastName,
                Email = u.Email,
                Phone = u.Phone ?? string.Empty,
                Role = u.Role,
                JoinDate = u.JoinDate,
                IsBanned = !u.IsActive,
                TotalLoans = u.Role == "Member" ? (int?)u.Loans.Count : null,
                TotalBorrows = u.Role == "Member" ? (int?)u.Reservations.Count : null,
                TotalFines = u.Role == "Member" ? (decimal?)u.Fines.Sum(f => f.Amount) : null
            }).ToList();

            if (!string.IsNullOrEmpty(roleFilter))
            {
                models = models.Where(m => m.Role.Equals(roleFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var viewModel = new UserManagementViewModel
            {
                Users = models,
                MemberCount = models.Count(m => m.Role == "Member"),
                StaffCount = models.Count(m => m.Role == "Staff"),
                SelectedRoleFilter = roleFilter ?? "member"
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Ban(string id)
        {
            Guid userId = Guid.Parse(id);
            var user = await _apiService.FindUserByIdAsync(userId);
            if (user == null) return NotFound();

            user.IsActive = false;
            var success = await _apiService.UpdateUserAsync(userId, user);
            if (!success)
            {
                TempData["Error"] = "Failed to ban user.";
                return RedirectToAction("Index");
            }

            TempData["Success"] = "User banned successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Unban(string id)
        {
            Guid userId = Guid.Parse(id);
            var user = await _apiService.FindUserByIdAsync(userId);
            if (user == null) return NotFound();

            user.IsActive = true;
            var success = await _apiService.UpdateUserAsync(userId, user);
            if (!success)
            {
                TempData["Error"] = "Failed to unban user.";
                return RedirectToAction("Index");
            }

            TempData["Success"] = "User unbanned successfully.";
            return RedirectToAction("Index");
        }


    }
}
