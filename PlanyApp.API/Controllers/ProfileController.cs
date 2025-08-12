using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanyApp.API.DTOs;
using PlanyApp.API.Models;
using PlanyApp.Repository.UnitOfWork;
using System.Security.Claims;
using static PlanyApp.API.Controllers.UsersController;
using BCrypt.Net;
using PlanyApp.Service.Interfaces;

namespace PlanyApp.API.Controllers
{
    [ApiController]
    [Route("api/profile")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserPackageService _userPackageService;

        public ProfileController(IUnitOfWork uow, IUserPackageService userPackageService)
        {
            _uow = uow;
            _userPackageService = userPackageService;
        }

        // GET: api/profile
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _uow.UserRepository.GetByIdAsync(int.Parse(userId));
            if (user == null)
                return NotFound(ApiResponse<object>.ErrorResponse("User not found"));

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
                UpdatedDate = user.UpdatedDate,
                DateOfBirth = user.DateOfBirth,
                City = user.City,
                MonthlyIncome = user.MonthlyIncome
            };

            // enrich with current package
            var current = await _userPackageService.GetCurrentPackageByUserIdAsync(user.UserId);
            if (current != null)
            {
                userDto.CurrentPackageId = current.PackageId;
                userDto.CurrentPackageName = current.PackageName;
                userDto.CurrentPackageStartDate = current.StartDate;
                userDto.CurrentPackageEndDate = current.EndDate;
            }
            return Ok(ApiResponse<UserDTO>.SuccessResponse(userDto));
        }

        // PUT: api/profile
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateUserRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _uow.UserRepository.GetByIdAsync(int.Parse(userId));
            if (user == null)
                return NotFound(ApiResponse<object>.ErrorResponse("User not found"));

            // Update only provided fields
            if (request.FullName != null) user.FullName = request.FullName;
            if (request.Phone != null) user.Phone = request.Phone;
            if (request.Address != null) user.Address = request.Address;
            if (request.Avatar != null) user.Avatar = request.Avatar;
            if (request.DateOfBirth.HasValue) user.DateOfBirth = request.DateOfBirth.Value;
            if (request.City != null) user.City = request.City;
            if (request.MonthlyIncome.HasValue) user.MonthlyIncome = request.MonthlyIncome.Value;

            await _uow.UserRepository.UpdateAsync(user);

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
                UpdatedDate = user.UpdatedDate,
                DateOfBirth = user.DateOfBirth,
                City = user.City,
                MonthlyIncome = user.MonthlyIncome
            };
            return Ok(ApiResponse<UserDTO>.SuccessResponse(userDto, "Profile updated successfully"));
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (request.NewPassword != request.ConfirmPassword)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("New password and confirmation password do not match."));
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _uow.UserRepository.GetByIdAsync(int.Parse(userId));
            if (user == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("User not found"));
            }

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Current password is incorrect"));
            }

            // Update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _uow.UserRepository.UpdateAsync(user);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Password changed successfully"));
        }
    }
} 