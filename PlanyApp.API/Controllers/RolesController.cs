using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanyApp.API.Models;
using PlanyApp.Repository.Models;
using PlanyApp.Repository.UnitOfWork;
using System;
using System.Linq;

namespace PlanyApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class RolesController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public RolesController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _uow.RoleRepository.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<Role>>.SuccessResponse(roles));
        }

        // GET: api/Roles/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var role = await _uow.RoleRepository.GetByIdAsync(id);
            if (role == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Role not found"));
            }

            return Ok(ApiResponse<Role>.SuccessResponse(role));
        }

        // POST: api/Roles
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Role role)
        {
            if (string.IsNullOrEmpty(role.Name))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Role name is required"));
            }

            // Check if role with same name exists
            var existingRole = await _uow.RoleRepository.FindAsync(r => r.Name == role.Name);
            if (existingRole.Any())
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Role with this name already exists"));
            }
            
            // The RoleId is now an IDENTITY column and should not be set here.
            // role.RoleId = 0; 
            _uow.RoleRepository.Add(role);
            await _uow.SaveAsync();

            return CreatedAtAction(nameof(Get), new { id = role.RoleId }, 
                ApiResponse<Role>.SuccessResponse(role, "Role created successfully"));
        }

        // PUT: api/Roles/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Role role)
        {
            if (id != role.RoleId)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch"));
            }

            var existingRole = await _uow.RoleRepository.GetByIdAsync(id);
            if (existingRole == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Role not found"));
            }

            // Check if updating to an existing name
            var nameExists = await _uow.RoleRepository.FindAsync(r => r.Name == role.Name && r.RoleId != id);
            if (nameExists.Any())
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Role with this name already exists"));
            }

            _uow.RoleRepository.Update(role);
            await _uow.SaveAsync();

            return Ok(ApiResponse<Role>.SuccessResponse(role, "Role updated successfully"));
        }

        // DELETE: api/Roles/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _uow.RoleRepository.GetByIdAsync(id);
            if (role == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Role not found"));
            }

            // Check if role is in use
            var users = await _uow.UserRepository.GetAllAsync();
            var usersWithRole = users.Where(u => u.RoleId == id);
            if (usersWithRole.Any())
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Cannot delete role as it is assigned to users"));
            }

            _uow.RoleRepository.Remove(role);
            await _uow.SaveAsync();

            return Ok(ApiResponse<object>.SuccessResponse(null, "Role deleted successfully"));
        }

        // POST: api/Roles/seed
        [HttpPost("seed")]
        public async Task<IActionResult> SeedRoles()
        {
            var defaultRoles = new List<Role>
            {
                new Role { Name = "admin", Description = "Administrator role with full access" },
                new Role { Name = "user", Description = "Standard user role" },
                new Role { Name = "moderator", Description = "Moderator role with limited administrative access" }
            };

            foreach (var role in defaultRoles)
            {
                var existingRole = await _uow.RoleRepository.FindAsync(r => r.Name == role.Name);
                if (!existingRole.Any())
                {
                    _uow.RoleRepository.Add(role);
                }
            }

            await _uow.SaveAsync();
            return Ok(ApiResponse<object>.SuccessResponse(defaultRoles, "Default roles seeded successfully"));
        }
    }
} 