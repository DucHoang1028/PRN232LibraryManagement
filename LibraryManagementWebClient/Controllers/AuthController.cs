using LibraryManagementWebClient.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using LibraryManagementWebClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

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

        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Index", "Admin");

                if (User.IsInRole("Staff"))
                    return RedirectToAction("Index", "Staff");

                if (User.IsInRole("Member"))
                    return RedirectToAction("Index", "Home");

                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(ViewModels.LoginViewModel model, string? returnUrl = null)
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

                HttpContext.Session.SetString("JWToken", loginResult.Token);

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
                    "Admin" => RedirectToAction("Index", "Admin"),
                    "Staff" => RedirectToAction("Index", "Home"),
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


        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Index", "Admin");

                if (User.IsInRole("Staff"))
                    return RedirectToAction("Index", "Home");

                if (User.IsInRole("Member"))
                    return RedirectToAction("Index", "Home");

                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Email) || !model.Email.EndsWith("@fpt.edu.vn", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("Email", "Email must be an FPT student email (ending in @fpt.edu.vn).");
                    return View(model);
                }

                var response = await _apiService.RegisterAsync(model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Registration successful! Please log in.";
                    return RedirectToAction(nameof(Login));
                }

                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Registration failed: {error}");
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
    }
}

namespace LibraryManagementWebClient.Models
{

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