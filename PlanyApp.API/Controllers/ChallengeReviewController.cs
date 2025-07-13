using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanyApp.API.Models;
using PlanyApp.Service.Dto.ChallengeReview;
using PlanyApp.Service.Interfaces;

[ApiController]
[Route("api/[controller]")]
[Authorize]
//[Authorize(Roles = "admin")]
public class ChallengeReviewController : ControllerBase
{
    private readonly IChallengeReviewService _challengeReviewService;

    public ChallengeReviewController(IChallengeReviewService challengeReviewService)
    {
        _challengeReviewService = challengeReviewService;
    }

    [HttpGet("pending-group-challenges")]
    public async Task<IActionResult> GetPendingGroupChallenges()
    {
        try
        {
            var data = await _challengeReviewService.GetPendingGroupChallengesAsync();
            return Ok(new ApiResponse<List<PendingGroupChallengeDto>>
            {
                Success = true,
                Message = "Lấy danh sách challenge nhóm cần duyệt thành công.",
                Data = data
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<PendingGroupChallengeDto>>
            {
                Success = false,
                Message = "Lỗi khi lấy danh sách challenge nhóm cần duyệt.",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    //===================================================
    [HttpPost("details")]
    [ProducesResponseType(typeof(ApiResponse<List<ImageDetailDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<ImageDetailDto>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDetails([FromBody] GetImageDetailsRequestDto request)
    {
        if (request.UserChallengeProgressIds == null || !request.UserChallengeProgressIds.Any())
            return BadRequest(new ApiResponse<List<ImageDetailDto>>
            {
                Success = false,
                Message = "userChallengeProgressIds không được rỗng."
            });

        try
        {
            var data = await _challengeReviewService.FetchImagesForReviewAsync(request);
            return Ok(new ApiResponse<List<ImageDetailDto>>
            {
                Success = true,
                Message = "Lấy chi tiết ảnh thành công.",
                Data = data
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<ImageDetailDto>>
            {
                Success = false,
                Message = "Lỗi khi lấy chi tiết ảnh.",
                Errors = new[] { ex.Message }
            });
        }
    }

    //===================================================
    /// <summary>
    /// Approve a selection of images for a challenge.
    /// </summary>
    [HttpPost("pending/approve")]
    public async Task<IActionResult> ApproveSelection([FromBody] ApproveImageSelectionRequestDto request)
    {
        if (request.UserChallengeProgressIds == null || !request.UserChallengeProgressIds.Any())
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Danh sách userChallengeProgressIds không được để trống."
            });
        }

        await _challengeReviewService.ApproveSelectionAsync(request);
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Duyệt hình thành công.",
            Data = null
        });
    }
}
