using Microsoft.AspNetCore.Mvc;
using PlanyApp.Service.Dto;
using PlanyApp.Service.Dto.Auth;
using PlanyApp.Service.Interfaces;
using System.Threading.Tasks;
using PlanyApp.API.Models;
using Microsoft.AspNetCore.Authorization;

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
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid model state",
                    Errors = ModelState
                });
            }
            var result = await _authService.RegisterAsync(registerDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid model state",
                    Errors = ModelState
                });
            }

            var result = await _authService.LoginAsync(loginDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDto googleLoginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid model state",
                    Errors = ModelState
                });
            }
            var result = await _authService.LoginWithGoogleAsync(googleLoginDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("activate-email")]
        public async Task<IActionResult> ActivateEmail([FromBody] ActivateEmailDto activateEmailDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid model state",
                    Errors = ModelState
                });
            }
            var result = await _authService.ActivateEmailAsync(activateEmailDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid model state",
                    Errors = ModelState
                });
            }
            var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);
            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid model state",
                    Errors = ModelState
                });
            }
            var result = await _authService.ResetPasswordAsync(resetPasswordDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("resend-activation")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendActivationEmail([FromBody] ResendActivationEmailDto resendActivationEmailDto)
        {
            var result = await _authService.ResendActivationEmailAsync(resendActivationEmailDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
} 