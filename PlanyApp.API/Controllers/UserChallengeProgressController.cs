using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanyApp.API.Models;
using PlanyApp.Service.Interfaces;
using PlanyApp.Service.Services;
using System.Security.Claims;

namespace PlanyApp.API.Controllers
{
    [ApiController]
    [Route("api/user-challenge-progress")]
    [Authorize]
    public class UserChallengeProgressController : ControllerBase
    {
        private readonly IUserChallengeProgressService _progressService;

        public UserChallengeProgressController(IUserChallengeProgressService progressService)
        {
            _progressService = progressService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProgress(int challengeId, int userPackageId)
        {
            //var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            //var progressId = await _progressService.CreateProgressAsync(challengeId, userPackageId, userId);

            //return Ok(new { userChallengeProgressId = progressId, status = "Started" });
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var progressId = await _progressService.CreateProgressAsync(challengeId, userPackageId, userId);

                var data = new
                {
                    userChallengeProgressId = progressId,
                    status = "Started"
                };

                return Ok(ApiResponse<object>.SuccessResponse(data, "Progress created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to create progress", ex.Message));
            }
        }

    }
}
