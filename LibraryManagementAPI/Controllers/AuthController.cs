using Contracts;
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
        public async Task<IActionResult> RegisterUser([FromBody]RegisterRequest user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterUserAsync(user);

            if (!result.Succeeded)
                return BadRequest(new { Message = result.ErrorMessage });

            return Ok(new { Message = "User registered successfully" });
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
