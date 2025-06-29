using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
        {
            var group = await _groupService.CreateGroupAsync(request);
            var result = new
            {
                group.GroupId,

            };
            return Ok(result);
        }

        /// <summary>
        /// Tạo link mời + QR code cho group
        /// </summary>
        [HttpGet("invite-links")]
        public async Task<IActionResult> GetInviteLink([FromQuery] int groupId)
        {
            if (groupId <= 0)
                return BadRequest(new { message = "Invalid groupId" });

            var result = await _groupService.GenerateInviteLinkAsync(groupId);

            return Ok(new
            {
                inviteLink = result.InviteLink,
                qrUrl = result.QrUrl
            });
        }

        /// <summary>
        /// Xử lý user join group bằng link mời
        /// </summary>
        [HttpPost("member-join")]
        public async Task<IActionResult> JoinGroup([FromBody] GroupInviteRequestDto request)
        {
            if (request.GroupId <= 0 || request.UserId <= 0 || string.IsNullOrEmpty(request.Sig) || request.Ts <= 0)
                return BadRequest(new { success = false, message = "Invalid request data" });

            var success = await _groupService.JoinGroupAsync(request);

            if (!success)
                return BadRequest(new { success = false, message = "Invalid or expired link, or already joined" });

            return Ok(new { success = true, message = "Joined successfully" });
        }
    }
}
