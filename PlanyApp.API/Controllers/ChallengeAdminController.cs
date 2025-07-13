using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanyApp.API.Models;
using PlanyApp.Service.Dto.Challenge;
using PlanyApp.Service.Interfaces;

namespace PlanyApp.API.Controllers
{
    [ApiController]
    [Route("api/challenge-admin")]
    [Authorize]
    //[Authorize(Roles = "admin")]
    public class ChallengeAdminController : ControllerBase
    {
        private readonly IChallengeService _challengeService;

        public ChallengeAdminController(IChallengeService challengeService)
        {
            _challengeService = challengeService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateChallenge([FromForm] ChallengeCreateWithFileDto dto)
        {
            try
            {
                var result = await _challengeService.CreateChallengeAsync(dto);
                return Ok(ApiResponse<ChallengeCreateResultDto>.SuccessResponse(result, "Challenge created successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("Tạo challenge thất bại", ex.Message));
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChallengeDetail(int id)
        {
            var result = await _challengeService.GetChallengeByIdAsync(id);
            if (result == null)
                return NotFound(ApiResponse<string>.ErrorResponse("Challenge không tồn tại"));

            return Ok(ApiResponse<ChallengeDetailDto>.SuccessResponse(result, "Lấy chi tiết challenge thành công."));
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchChallenge(int id, [FromForm] ChallengePatchDto dto)
        {
            try
            {
                var success = await _challengeService.PatchChallengeAsync(id, dto);
                if (!success)
                    return NotFound(ApiResponse<string>.ErrorResponse("Challenge không tồn tại"));

                return Ok(ApiResponse<string>.SuccessResponse(null, "Cập nhật challenge thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("Cập nhật thất bại", ex.Message));
            }
        }
        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleChallengeStatus(int id)
        {
            try
            {
                var result = await _challengeService.ToggleChallengeStatusAsync(id);
                if (result == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Challenge không tồn tại."));

                return Ok(ApiResponse<object>.SuccessResponse(new
                {
                    isActive = result
                }, "Trạng thái challenge đã được chuyển đổi."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("Không thể cập nhật trạng thái challenge.", ex.Message));
            }
        }
        [HttpGet("paged-filtered")]
        public async Task<IActionResult> GetFilteredPagedChallenges([FromQuery] ChallengeListFilterDto filter)
        {
            try
            {
                var (items, totalCount) = await _challengeService.GetFilteredChallengesAsync(filter);

                return Ok(ApiResponse<object>.SuccessResponse(new
                {
                    Items = items,
                    TotalCount = totalCount,
                    filter.PageIndex,
                    filter.PageSize
                }, "Lấy danh sách challenge thành công."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Lỗi khi lấy danh sách challenge.", ex.Message));
            }
        }



    }

}
