using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanyApp.API.DTOs;
using PlanyApp.API.Models;
using PlanyApp.Repository.Models;
using PlanyApp.Repository.UnitOfWork;
using PlanyApp.Service.Dto.UserPackage;
using PlanyApp.Service.Interfaces;
using PlanyApp.Service.Services;
using System.Linq;
using System.Security.Claims;
using BC = BCrypt.Net.BCrypt;

namespace PlanyApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserPackageService _userPackageService;
        private readonly IUserService _userService;

        public class UpdateUserRequest
        {
            public string? FullName { get; set; }
            public string? Phone { get; set; }
            public string? Address { get; set; }
            public string? Avatar { get; set; }
        }

        public class ChangePasswordRequest
        {
            public string CurrentPassword { get; set; } = string.Empty;
            public string NewPassword { get; set; } = string.Empty;
        }

        public class AssignRoleRequest
        {
            public int RoleId { get; set; }
        }

        public UsersController(IUnitOfWork uow, IUserPackageService userPackageService, IUserService userService)
        {
            _uow = uow;
            _userPackageService = userPackageService;
            _userService = userService;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _uow.UserRepository.GetAllAsync();
            var userDtos = users.Select(u => new UserDTO
            {
                UserId = u.UserId,
                FullName = u.FullName ?? string.Empty,
                Email = u.Email ?? string.Empty,
                Phone = u.Phone,
                Address = u.Address,
                Avatar = u.Avatar,
                RoleId = u.RoleId,
                EmailVerified = u.EmailVerified,
                CreatedDate = u.CreatedDate,
                UpdatedDate = u.UpdatedDate
            });
            return Ok(ApiResponse<IEnumerable<UserDTO>>.SuccessResponse(userDtos));
        }

        // GET: api/Users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var requestingUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Check if user is requesting their own data or is an admin
            if (requestingUserId != id.ToString()
                && !User.IsInRole("admin"))
            {
                return Forbid();
            }

            var user = await _uow.UserRepository.GetByIdAsync(id);
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
                UpdatedDate = user.UpdatedDate
            };
            return Ok(ApiResponse<UserDTO>.SuccessResponse(userDto));
        }

        // PUT: api/Users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request)
        {
            var requestingUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Check if user is updating their own data or is an admin
            if (requestingUserId != id.ToString()
                && !User.IsInRole("admin"))
            {
                return Forbid();
            }

            var user = await _uow.UserRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound(ApiResponse<object>.ErrorResponse("User not found"));

            // Update only provided fields
            if (request.FullName != null) user.FullName = request.FullName;
            if (request.Phone != null) user.Phone = request.Phone;
            if (request.Address != null) user.Address = request.Address;
            if (request.Avatar != null) user.Avatar = request.Avatar;

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
                UpdatedDate = user.UpdatedDate
            };
            return Ok(ApiResponse<UserDTO>.SuccessResponse(userDto, "User updated successfully"));
        }

        // DELETE: api/Users/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _uow.UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("User not found"));
            }

            // Prevent deleting the last admin
            if (user.RoleId != null)
            {
                var role = await _uow.RoleRepository.GetByIdAsync(user.RoleId.Value);
                if (role?.Name == "admin")
                {
                    var users = await _uow.UserRepository.GetAllAsync();
                    var adminCount = users.Count(u => u.Role?.Name == "admin");
                    if (adminCount <= 1)
                    {
                        return BadRequest(ApiResponse<object>.ErrorResponse("Cannot delete the last admin user"));
                    }
                }
            }

            await _uow.UserRepository.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "User deleted successfully"));
        }

        // PUT: api/Users/{id}/role
        [HttpPut("{id}/role")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AssignRole(int id, [FromBody] AssignRoleRequest request)
        {
            var user = await _uow.UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("User not found"));
            }

            var role = await _uow.RoleRepository.GetByIdAsync(request.RoleId);
            if (role == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Role not found"));
            }

            // Prevent removing the last admin
            if (user.RoleId != null)
            {
                var currentRole = await _uow.RoleRepository.GetByIdAsync(user.RoleId.Value);
                if (currentRole?.Name == "admin" && role.Name != "admin")
                {
                    var users = await _uow.UserRepository.GetAllAsync();
                    var adminCount = users.Count(u => u.Role?.Name == "admin");
                    if (adminCount <= 1)
                    {
                        return BadRequest(ApiResponse<object>.ErrorResponse("Cannot remove the last admin role"));
                    }
                }
            }

            user.RoleId = request.RoleId;
            await _uow.UserRepository.UpdateAsync(user);

            return Ok(ApiResponse<object>.SuccessResponse(new { message = "Role assigned successfully" }));
        }

        // PUT: api/Users/{id}/password
        [HttpPut("{id}/password")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
        {
            var requestingUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Only allow users to change their own password (except admins)
            if (requestingUserId != id.ToString()
                && !User.IsInRole("admin"))
            {
                return Forbid();
            }

            var user = await _uow.UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("User not found"));
            }

            // Verify current password
            if (!BC.Verify(request.CurrentPassword, user.PasswordHash))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Current password is incorrect"));
            }

            // Update password
            user.PasswordHash = BC.HashPassword(request.NewPassword);
            await _uow.UserRepository.UpdateAsync(user);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Password changed successfully"));
        }
        //------- 2 ham nay cua nguyen

        [HttpGet("{userId}/packages")]
        public async Task<IActionResult> GetUserPackages(int userId)
        {
            var result = await _userPackageService.GetPackageIdsByUserIdAsync(userId);
            return Ok(ApiResponse<List<ResponseListUserPackage>>.SuccessResponse(result));
        }
        [HttpGet("{userId}/points")]
        public async Task<IActionResult> GetUserPoints(int userId)
        {
            var points = await _userService.GetUserPointsAsync(userId);
            if (points == null)
                return NotFound(ApiResponse<string>.ErrorResponse("User not found"));

            return Ok(ApiResponse<int>.SuccessResponse(points.Value));
        }


    }
}
