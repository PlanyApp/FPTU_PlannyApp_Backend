using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanyApp.API.Models;
using PlanyApp.Service.Dto.ImageS3;
using PlanyApp.Service.Dto.UserChallengeProgress;
using PlanyApp.Service.Interfaces;

[ApiController]
[Route("api/user-challenge-proof")]
[Authorize]
public class UserChallengeProofController : ControllerBase
{
    private readonly IUserChallengeProofService _proofService;

    public UserChallengeProofController(IUserChallengeProofService proofService)
    {
        _proofService = proofService;
    }

    [HttpPost("upload")]
    [ProducesResponseType(typeof(ImageS3Dto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadProof([FromForm] UploadProofImageRequestDto request)
    {
        var result = await _proofService.UploadProofAsync(request);

        if (result.IsSuccess)
        {
            return Ok(ApiResponse<ImageS3Dto>.SuccessResponse(result.Data, result.Message));
        }

        return BadRequest(ApiResponse<ImageS3Dto>.ErrorResponse(result.Message, result.Errors));
    }
}
