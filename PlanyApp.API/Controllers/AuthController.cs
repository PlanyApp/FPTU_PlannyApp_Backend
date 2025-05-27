using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PlanyApp.API.DTOs;
using PlanyApp.API.Models;
using PlanyApp.Repository.Models;
using PlanyApp.Repository.UnitOfWork;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace PlanyApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _configuration;

        public class LoginRequest
        {
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email format")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
            public string Password { get; set; } = string.Empty;
        }

        public class RegisterRequest
        {
            [Required(ErrorMessage = "Full name is required")]
            [StringLength(255, ErrorMessage = "Full name must be between 2 and 255 characters", MinimumLength = 2)]
            public string FullName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email format")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
            public string Password { get; set; } = string.Empty;

            public string? Phone { get; set; }
            public string? Address { get; set; }
            public string? Avatar { get; set; }
        }

        public AuthController(IUnitOfWork uow, IConfiguration configuration)
        {
            _uow = uow;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Validation failed",
                    ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                ));
            }

            // Check if email already exists
            if (await _uow.UserRepository.EmailExistsAsync(request.Email))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Email already registered"));
            }

            // Get default user role
            var userRole = (await _uow.RoleRepository.FindAsync(r => r.Name == "user")).FirstOrDefault();
            if (userRole == null)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Default user role not found"));
            }

            // Create new user
            var user = new User
            {
                UserId = Guid.NewGuid().ToString(),
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = BC.HashPassword(request.Password),
                Phone = request.Phone,
                Address = request.Address,
                Avatar = request.Avatar,
                RoleId = userRole.RoleId,
                EmailVerified = false,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            await _uow.UserRepository.AddAsync(user);

            // Generate token for the new user
            var token = GenerateJwtToken(user);
            var userDto = new UserDTO
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Avatar = user.Avatar,
                RoleId = user.RoleId,
                EmailVerified = user.EmailVerified,
                CreatedDate = user.CreatedDate,
                UpdatedDate = user.UpdatedDate
            };

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                token = token,
                user = userDto
            }, "Registration successful"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Validation failed",
                    ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                ));
            }

            var user = await _uow.UserRepository.GetByEmailAsync(request.Email);
            
            if (user == null)
                return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid email or password"));

            if (!BC.Verify(request.Password, user.PasswordHash))
                return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid email or password"));

            var token = GenerateJwtToken(user);
            var userDto = new UserDTO
            {
                UserId = user.UserId,
                FullName = user.FullName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Phone = user.Phone,
                Address = user.Address,
                Avatar = user.Avatar,
                RoleId = user.RoleId,
                EmailVerified = user.EmailVerified,
                CreatedDate = user.CreatedDate,
                UpdatedDate = user.UpdatedDate
            };

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                token = token,
                user = userDto
            }, "Login successful"));
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["JWT:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT:Key is not configured");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role?.Name ?? "user")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JWT:ExpiryInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
} 