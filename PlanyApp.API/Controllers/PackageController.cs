using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanyApp.API.Models;
using PlanyApp.Service.Dto.Package;
using PlanyApp.Service.Interfaces;

namespace PlanyApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        /// <summary>
        /// Get list packages
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _packageService.GetAllPackagesAsync();
            return Ok(ApiResponse<List<PackageDto>>.SuccessResponse(data, "Lấy danh sách package thành công."));
        }

        /// <summary>
        /// Get a package by id
        /// </summary>
        [HttpGet("{packageId}")]
        public async Task<IActionResult> GetById(int packageId)
        {
            var data = await _packageService.GetByIdAsync(packageId);
            if (data == null) return NotFound(ApiResponse<string>.ErrorResponse("Package not found"));
            return Ok(ApiResponse<PackageDto>.SuccessResponse(data));
        }

        /// <summary>
        /// Create a new package
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreatePackageRequestDto request)
        {
            var created = await _packageService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { packageId = created.PackageId }, ApiResponse<PackageDto>.SuccessResponse(created, "Created"));
        }

        /// <summary>
        /// Update a package
        /// </summary>
        [HttpPut("{packageId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int packageId, [FromBody] UpdatePackageRequestDto request)
        {
            var updated = await _packageService.UpdateAsync(packageId, request);
            if (updated == null) return NotFound(ApiResponse<string>.ErrorResponse("Package not found"));
            return Ok(ApiResponse<PackageDto>.SuccessResponse(updated, "Updated"));
        }

        /// <summary>
        /// Delete a package
        /// </summary>
        [HttpDelete("{packageId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int packageId)
        {
            var ok = await _packageService.DeleteAsync(packageId);
            if (!ok) return NotFound(ApiResponse<string>.ErrorResponse("Package not found"));
            return NoContent();
        }
    }
}
