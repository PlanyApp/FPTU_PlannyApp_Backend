using Microsoft.AspNetCore.Mvc;
using PlanyApp.API.Models;
using PlanyApp.Service.Dto.Group;
using PlanyApp.Service.Dto.UserPackage;
using PlanyApp.Service.Interfaces;
using PlanyApp.Service.Services;

namespace PlanyApp.API.Controllers
{
    [ApiController]
    [Route("api/challenge")]
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
        [HttpGet("challenge/{challengeId}/description")]
        public async Task<IActionResult> GetChallengeDescription(int challengeId)
        {
            var description = await _challengeService.GetChallengeDescriptionAsync(challengeId);

            if (description == null)
                return NotFound(ApiResponse<string>.ErrorResponse("Challenge not found"));

            return Ok(ApiResponse<string>.SuccessResponse(description));
        }



    }
}
