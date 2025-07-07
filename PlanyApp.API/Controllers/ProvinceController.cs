using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanyApp.Service.Interfaces;
using PlanyApp.API.Models;
using PlanyApp.Repository.Models;
using PlanyApp.Service.Dto.Province;

namespace PlanyApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class ProvinceController:ControllerBase
    {
        private readonly IProvinceService _provinceService;
        public ProvinceController(IProvinceService provinceService)
        {
            _provinceService = provinceService;
        }
        [HttpGet("list")]
        public IActionResult GetListProvinces()
        {
            
            var provinces = _provinceService.GetListProvinces();

            if (provinces == null || !provinces.Any())
            {
                return NotFound(ApiResponse<object>.ErrorResponse("No provinces found."));
            }
            return Ok(ApiResponse<List<ProvinceDto>>.SuccessResponse(provinces));
        }
    }
}
