using Microsoft.AspNetCore.Mvc;
using PlanyApp.API.Models;
using PlanyApp.Service.Dto.Group;
using PlanyApp.Service.Interfaces;

namespace PlanyApp.API.Controllers
{
    [ApiController]
    [Route("api/group")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        //[HttpPost("")]
        //public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
        //{
        //    if (string.IsNullOrWhiteSpace(request.GroupName))
        //    {
        //        return BadRequest(ApiResponse<string>.ErrorResponse("Tên nhóm không được để trống"));
        //    }

        //    var group = await _groupService.CreateGroupAsync(request);

        //    if (group == null)
        //    {
        //        return BadRequest(ApiResponse<string>.ErrorResponse("Không thể tạo nhóm"));
        //    }

        //    var result = new
        //    {
        //        group.GroupId
        //        // Thêm field khác nếu muốn
        //    };

        //    return Ok(ApiResponse<object>.SuccessResponse(result, "Tạo nhóm thành công"));
        //}

        /// <summary>
        /// Tạo link mời + QR code cho group
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
        /// Xử lý user join group bằng link mời
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
    }
}
