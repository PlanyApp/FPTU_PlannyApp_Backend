using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanyApp.API.DTOs;
using PlanyApp.API.Models;
using PlanyApp.Service.Dto;
using PlanyApp.Service.Interfaces;

namespace PlanyApp.API.Controllers
{
    [ApiController]
    [Route("api/chat")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChatRequestDto request)
        {
            if (request.Messages == null || !request.Messages.Any())
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Messages cannot be empty."));
            }

            var responseContent = await _chatService.GetChatCompletionAsync(request.Messages);
            var responseDto = new ChatMessageDto { Role = "assistant", Content = responseContent };
            
            return Ok(ApiResponse<ChatMessageDto>.SuccessResponse(responseDto));
        }
    }
} 