using LibraryManagementWebClient.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using LibraryManagementWebClient.Services;

namespace LibraryManagementWebClient.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILibraryApiService _apiService;

        public AuthController(ILibraryApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                try
                {
                    // Get member by email from API
                    var member = await _apiService.GetMemberByEmailAsync(model.Email);
                    
                    if (member != null)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, member.Email),
                            new Claim(ClaimTypes.Role, member.Role), // Get role from member data
                            new Claim(ClaimTypes.NameIdentifier, member.MemberId.ToString()),
                            new Claim("FirstName", member.FirstName),
                            new Claim("LastName", member.LastName),
                            new Claim("MemberId", member.MemberId.ToString()),
                            new Claim("LibraryCardNumber", member.LibraryCardNumber),
                            new Claim("Phone", member.Phone ?? ""),
                            new Claim("Address", member.Address ?? ""),
                            new Claim("JoinDate", member.JoinDate.ToString("yyyy-MM-dd"))
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        TempData["Success"] = $"Welcome back, {member.FirstName}! You are logged in as {member.Role}.";

                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }

                        return RedirectToAction("Index", "Customer");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Email not found. Please check your email or register for an account.");
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Error connecting to the server. Please try again.");
                }
            }

            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Simple registration logic - in real app, save to database
                if (RegisterUser(model))
                {
                    TempData["Success"] = "Registration successful! Please log in.";
                    return RedirectToAction(nameof(Login));
                }

                ModelState.AddModelError(string.Empty, "Registration failed. Email might already be in use.");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Customer");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        private bool RegisterUser(RegisterViewModel model)
        {
            // Simple demo registration - replace with database save
            // For demo purposes, we'll just return true
            return true;
        }
    }
}

namespace LibraryManagementWebClient.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Password field commented out for now
        /*
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        */

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; } = "Guest";
    }
} 