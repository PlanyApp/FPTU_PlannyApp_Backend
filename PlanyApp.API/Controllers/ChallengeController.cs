using Microsoft.AspNetCore.Mvc;
using PlanyApp.API.Models;
using PlanyApp.Service.Dto.Challenge;
using PlanyApp.Service.Dto.Group;
using PlanyApp.Service.Dto.UserPackage;
using PlanyApp.Service.Interfaces;
using PlanyApp.Service.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace PlanyApp.API.Controllers
{
    [ApiController]
    [Route("api/challenge")]
    [Authorize]
    public class ChallengeController : ControllerBase
    {
        private readonly IChallengeService _challengeService;
        private readonly IUserPackageService _userPackageService;
        public ChallengeController(IChallengeService challengeService, IUserPackageService userPackageService)
        {
            _challengeService = challengeService;
            _userPackageService = userPackageService;
        }
        [HttpGet]
        public async Task<IActionResult> GetListChallenge(int packageId, int provinceId)
        {
            var challenges = await _challengeService.GetChallengesByPackageIdAsync(packageId, provinceId);
            var result = new
            {
                challenges = challenges.Select(c => new
                {
                    c.ChallengeId,
                    c.Name,
                    c.Description,
                    c.PackageId
                }).ToList()

            };
            return Ok(ApiResponse<object>.SuccessResponse( result, "Lấy challenge thành công"));
        }
        [HttpGet("/{challengeId}/description")]
        public async Task<IActionResult> GetChallengeDescription(int challengeId)
        {
            var description = await _challengeService.GetChallengeDescriptionAsync(challengeId);

            if (description == null)
                return NotFound(ApiResponse<string>.ErrorResponse("Challenge not found"));

            return Ok(ApiResponse<string>.SuccessResponse(description));
        }

        [HttpGet("{challengeId}/group/{groupId}/images")]
        public async Task<IActionResult> GetChallengeImagesProgress(int challengeId, int groupId, int userPackageId)
        {
            if (challengeId <= 0 || groupId <= 0)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("ChallengeId và GroupId phải lớn hơn 0."));
            }

            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdStr, out var currentUserId))
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse("Không thể xác định người dùng hiện tại."));
                }

                var data = await _challengeService.GetGroupChallengeImagesAsync(challengeId, groupId, currentUserId, userPackageId);
                return Ok(ApiResponse<ProgressChallengeImageListDto>.SuccessResponse(data, "Lấy danh sách hình ảnh thành công."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Đã xảy ra lỗi hệ thống.", ex.Message));
            }
        }
        [HttpGet("{challengeId}/personal/progress")]
        public async Task<IActionResult> GetPersonalChallengeProgress(int challengeId, int userPackageId)
        {
            if (challengeId <= 0)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("ChallengeId phải lớn hơn 0."));
            }

            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdStr, out var currentUserId))
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse("Không thể xác định người dùng hiện tại."));
                }

                var data = await _challengeService.GetPersonalChallengeProgressAsync(challengeId, currentUserId, userPackageId);
                return Ok(ApiResponse<PersonalChallengeProgressDto>.SuccessResponse(data, "Lấy tiến độ thử thách cá nhân thành công."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Đã xảy ra lỗi hệ thống.", ex.Message));
            }
        }


    }
}
