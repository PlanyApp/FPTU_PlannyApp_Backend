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
            try
            {
                var data = await _packageService.GetAllPackagesAsync();
                return Ok(new ApiResponse<List<PackageDto>>
                {
                    Success = true,
                    Message = "Lấy danh sách package thành công.",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                // Có thể log ex ở đây
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Đã có lỗi xảy ra khi lấy danh sách package.",
                    Data = ex.Message
                });
            }
        }
    }
}
