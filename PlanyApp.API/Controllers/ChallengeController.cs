using Microsoft.AspNetCore.Mvc;
using PlanyApp.API.Models;
using PlanyApp.Service.Dto.Group;
using PlanyApp.Service.Interfaces;
using PlanyApp.Service.Services;

namespace PlanyApp.API.Controllers
{
    [ApiController]
    [Route("api/challenge")]
    public class ChallengeController : ControllerBase
    {
        private readonly IChallengeService _challengeService;
        public ChallengeController(IChallengeService challengeService)
        {
            _challengeService = challengeService;
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
    }
}
