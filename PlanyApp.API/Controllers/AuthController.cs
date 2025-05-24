using Microsoft.AspNetCore.Mvc;
using PlanyApp.Service.Dto.Auth;
using PlanyApp.Service.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PlanyApp.API.Controllers
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
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(registerDto);

            if (!result.Success)
            {
                return BadRequest(new { Errors = result.Errors });
            }

            return Ok(result); // Contains UserInfo and Token
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(loginDto);

            if (!result.Success)
            {
                // For login, typically return Unauthorized or a generic bad request 
                // to avoid confirming if an email exists or not.
                return Unauthorized(new { Errors = result.Errors }); 
            }

            return Ok(result); // Contains UserInfo and Token
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDto googleLoginDto)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(googleLoginDto.IdToken))
            {
                return BadRequest(new { Errors = new List<string> { "Google ID token is required." }});
            }

            var result = await _authService.LoginWithGoogleAsync(googleLoginDto);

            if (!result.Success)
            {
                // Unauthorized or BadRequest depending on the nature of the error from the service
                return Unauthorized(new { Errors = result.Errors }); 
            }

            return Ok(result); // Contains UserInfo and Token
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);
            // Always return OK to prevent email enumeration, even if the service internally handles errors.
            return Ok(new { result.Message }); 
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.ResetPasswordAsync(resetPasswordDto);

            if (!result.Success)
            {
                return BadRequest(new { Errors = result.Errors });
            }

            return Ok(new { result.Message });
        }

        // Example of a protected endpoint (you can add this to any controller)
        // [HttpGet("me")]
        // [Authorize] // Requires a valid JWT
        // public IActionResult GetCurrentUser()
        // {
        //     var userId = User.FindFirstValue("UserId"); // From JWT claims
        //     var email = User.FindFirstValue(ClaimTypes.Email);
        //     // You can use this info to fetch more user details if needed
        //     return Ok(new { UserId = userId, Email = email });
        // }
    }
} 