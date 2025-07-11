﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanyApp.API.Models;
using PlanyApp.Service.Dto.Group;
using PlanyApp.Service.Interfaces;

namespace PlanyApp.API.Controllers
{
    [ApiController]
    [Route("api/group")]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        /// <summary>
        /// Create QR code adn invite link for group
        /// </summary>
        [HttpGet("invite-links")]
        public async Task<IActionResult> GetInviteLink([FromQuery] int groupId)
        {
            
            if (groupId <= 0)
                return BadRequest(ApiResponse<string>.ErrorResponse("GroupId không hợp lệ"));

            var result = await _groupService.GenerateInviteLinkAsync(groupId);

            if (result == null)
                return NotFound(ApiResponse<string>.ErrorResponse("Không tìm thấy group hoặc không thể tạo link mời"));

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                result.InviteLink,
                result.QrUrl
            }, "Lấy link mời thành công"));
        }

        /// <summary>
        /// Join a group
        /// </summary>
        [HttpPost("member-join")]
        public async Task<IActionResult> JoinGroup([FromBody] GroupInviteRequestDto request)
        {
            if (request.GroupId <= 0 || request.UserId <= 0 || string.IsNullOrEmpty(request.Sig) || request.Ts <= 0)
                return BadRequest(ApiResponse<string>.ErrorResponse("Dữ liệu yêu cầu không hợp lệ"));

            var success = await _groupService.JoinGroupAsync(request);

            if (!success)
                return BadRequest(ApiResponse<string>.ErrorResponse("Link mời không hợp lệ, đã hết hạn hoặc bạn đã tham gia"));

            return Ok(ApiResponse<string>.SuccessResponse(null, "Tham gia nhóm thành công"));
        }
        /// <summary>
        /// Get details of a group
        /// </summary>
        [HttpGet("{groupId}/details")]
        public async Task<IActionResult> GetGroupDetails(int groupId)
        {
            var result = await _groupService.GetGroupDetailsAsync(groupId);
            if (result == null)
                return NotFound();

            return Ok(ApiResponse<GroupDetailDto>.SuccessResponse(result));

        }

        /// <summary>
        /// Rename a group
        /// </summary>
        [HttpPut("rename")]
        public async Task<IActionResult> UpdateGroupName([FromBody] RequestUpdateGroupName request)
        {
            if (request.GroupId <= 0 || string.IsNullOrWhiteSpace(request.NewName))
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("Dữ liệu không hợp lệ"));
            }

            try
            {
                var success = await _groupService.UpdateGroupNameAsync(request);
                if (!success)
                    return NotFound(ApiResponse<string>.ErrorResponse("Không tìm thấy nhóm"));

                return Ok(ApiResponse<string>.SuccessResponse(null, "Đổi tên nhóm thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

    }
}
