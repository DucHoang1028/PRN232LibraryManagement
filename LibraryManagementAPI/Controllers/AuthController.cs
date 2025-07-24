using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace LibraryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.RegisterUserAsync(model);

            if (!result.Succeeded)
                return BadRequest(new { success = false, message = result.ErrorMessage });

            return Ok(new { success = true, message = "Registration successful!" });
        }



        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginRequest login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.LoginUserAsync(login);

            if (!response.Success)
            {
                return Unauthorized(new { Message = response.Message });
            }

            return Ok(new
            {
                Token = response.Token,
                ExpiresAt = response.Expiration,
                Role = response.Role,
                Message = response.Message,
                Member = response.Member
            });
        }



    }
}
