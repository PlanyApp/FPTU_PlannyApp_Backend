// API/Controllers/PersonalChallengeApprovalController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanyApp.API.Models;
using PlanyApp.Service.Dto.Challenge;
using PlanyApp.Service.Dto.ChallengeReview;
using PlanyApp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanyApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class PersonalChallengeApprovalController : ControllerBase
    {
        private readonly IPersonalChallengeApprovalService _service;

        public PersonalChallengeApprovalController(IPersonalChallengeApprovalService service)
        {
            _service = service;
        }

        [HttpGet("pending-images")]
        public async Task<IActionResult> GetPending()
        {
            try
            {
                var dtos = await _service.GetPendingImagesAsync();
                return Ok(ApiResponse<List<PendingImageListDto>>.SuccessResponse(dtos));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<PendingImageListDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("approve/{id}")]
        public async Task<IActionResult> Approve(int id, [FromBody] string? notes = null)
        {
            try
            {
                var ok = await _service.ApproveImageAsync(id, notes);
                if (!ok)
                    // gọi static ErrorResponse trên ApiResponse<object>
                    return BadRequest(ApiResponse<object>.ErrorResponse("Progress không tồn tại hoặc không hợp lệ"));

                // gọi static SuccessResponse trên ApiResponse<object>
                return Ok(ApiResponse<object>.SuccessResponse(null, "Duyệt ảnh thành công."));
            }
            catch (Exception ex)
            {
                // gọi static ErrorResponse trên ApiResponse<object>
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
        [HttpPost("reject/{id}")]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                var ok = await _service.RejectImageAsync(id);
                if (!ok)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Progress không tồn tại hoặc không hợp lệ"));

                return Ok(ApiResponse<object>.SuccessResponse(null, "Từ chối ảnh thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
    }
}
