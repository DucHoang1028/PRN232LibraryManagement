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
        private readonly ILibraryApiService _libraryApiService;
        private readonly IApiService _apiService;

        public AuthController(ILibraryApiService apiService, IApiService apiService1)
        {
            _libraryApiService = apiService;
            _apiService = apiService1;
        }

        public IActionResult Login(string? returnUrl = null)
        {
            // Keep track of where the user should be redirected after login
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var loginResult = await _apiService.AuthenticateAsync(model.Email, model.Password);

                if (loginResult == null || loginResult.Member == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    return View(model);
                }

                var member = loginResult.Member;

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, member.Email),
            new Claim(ClaimTypes.Role, member.Role),
            new Claim(ClaimTypes.NameIdentifier, member.MemberId.ToString()),
            new Claim("FirstName", member.FirstName),
            new Claim("LastName", member.LastName),
            new Claim("MemberId", member.MemberId.ToString()),
            new Claim("LibraryCardNumber", member.LibraryCardNumber),
            new Claim("Phone", member.Phone ?? ""),
            new Claim("Address", member.Address ?? ""),
            new Claim("JoinDate", member.JoinDate.ToString("yyyy-MM-dd"))
        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    authProperties
                );

                TempData["Success"] = $"Welcome back, {member.FirstName}! You are logged in as {member.Role}.";

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return member.Role switch
                {
                    "Admin" => RedirectToAction("Index", "Home"),
                    "Staff" => RedirectToAction("Index", "Dashboard"),
                    "Member" => RedirectToAction("Index", "Home"),
                    "Guest" => RedirectToAction("Guest", "AccessDenied"),
                    _ => RedirectToAction("Index", "Home")
                };
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Error connecting to the server. Please try again.");
                return View(model);
            }
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

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

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

    public class LoginResult
    {
        public bool IsSuccess { get; set; }
        public Member Member { get; set; }
        public string Token { get; set; } // if you want JWT too
    }

}