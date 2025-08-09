using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanyApp.API.DTOs;
using PlanyApp.API.Models;
using PlanyApp.Service.Dto;
using PlanyApp.Service.Interfaces;
using System.Security.Claims;
using System.Collections.Concurrent;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace PlanyApp.API.Controllers
{
    /// <summary>
    /// AI-powered travel planning chat system
    /// </summary>
    [ApiController]
    [Route("api/chat")]
    [Authorize]
    [Produces("application/json")]
    [SwaggerTag("AI travel planning chat with plan generation")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IConversationService _conversationService;
        
        // Thread-safe dictionary to store pending plan confirmations
        private static readonly ConcurrentDictionary<string, PlanSuggestion> _pendingPlans = new();

        public ChatController(IChatService chatService, IConversationService conversationService)
        {
            _chatService = chatService;
            _conversationService = conversationService;
        }

        /// <summary>
        /// Start new AI conversation
        /// </summary>
        /// <param name="request">Initial message</param>
        /// <returns>AI response with conversation ID</returns>
        [HttpPost("start")]
        [SwaggerOperation(Summary = "Start AI conversation", OperationId = "StartConversation")]
        [ProducesResponseType(typeof(ApiResponse<EnhancedChatResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StartConversation([FromBody] StartConversationDto request)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var response = await _chatService.StartConversationAsync(request.InitialMessage, userId);
                
                // Store pending plan suggestion
                if (response.PlanSuggestion != null && response.ConfirmationId != null)
                {
                    _pendingPlans[response.ConfirmationId] = response.PlanSuggestion;
                    CleanupExpiredConfirmations();
                }
                
                return Ok(ApiResponse<EnhancedChatResponseDto>.SuccessResponse(response));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse($"Error starting conversation: {ex.Message}"));
            }
        }

        /// <summary>
        /// Continue existing conversation
        /// </summary>
        /// <param name="request">Conversation ID and new message</param>
        /// <returns>AI response maintaining context</returns>
        [HttpPost("continue")]
        [SwaggerOperation(Summary = "Continue conversation", OperationId = "ContinueConversation")]
        [ProducesResponseType(typeof(ApiResponse<EnhancedChatResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ContinueConversation([FromBody] ContinueConversationDto request)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var response = await _chatService.ContinueConversationAsync(request.ConversationId, request.Message, userId);
                
                // Store pending plan suggestion
                if (response.PlanSuggestion != null && response.ConfirmationId != null)
                {
                    _pendingPlans[response.ConfirmationId] = response.PlanSuggestion;
                    CleanupExpiredConfirmations();
                }
                
                return Ok(ApiResponse<EnhancedChatResponseDto>.SuccessResponse(response));
            }
            catch (ArgumentException)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Conversation not found or access denied."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse($"Error continuing conversation: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get user's conversations
        /// </summary>
        /// <returns>List of conversations</returns>
        [HttpGet("conversations")]
        [SwaggerOperation(Summary = "Get conversations", OperationId = "GetUserConversations")]
        [ProducesResponseType(typeof(ApiResponse<List<ConversationHistoryDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserConversations()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var conversations = await _conversationService.GetUserConversationsAsync(userId);
            return Ok(ApiResponse<List<ConversationHistoryDto>>.SuccessResponse(conversations));
        }

        /// <summary>
        /// Get conversation by ID
        /// </summary>
        /// <param name="conversationId">Conversation ID</param>
        /// <returns>Complete conversation history</returns>
        [HttpGet("conversations/{conversationId}")]
        [SwaggerOperation(Summary = "Get conversation", OperationId = "GetConversation")]
        [ProducesResponseType(typeof(ApiResponse<ConversationHistoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetConversation(int conversationId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var conversation = await _conversationService.GetConversationAsync(conversationId, userId);
            
            if (conversation == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Conversation not found."));
            }
            
            return Ok(ApiResponse<ConversationHistoryDto>.SuccessResponse(conversation));
        }

        /// <summary>
        /// Delete conversation
        /// </summary>
        /// <param name="conversationId">Conversation ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("conversations/{conversationId}")]
        [SwaggerOperation(Summary = "Delete conversation", OperationId = "DeleteConversation")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteConversation(int conversationId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var deleted = await _conversationService.DeleteConversationAsync(conversationId, userId);
            
            if (!deleted)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Conversation not found."));
            }
            
            return Ok(ApiResponse<object>.SuccessResponse(null, "Conversation deleted successfully."));
        }

        /// <summary>
        /// Chat with AI (Legacy endpoint)
        /// </summary>
        /// <param name="request">Message history</param>
        /// <returns>AI response with plan suggestions</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Chat with AI", OperationId = "Chat")]
        [ProducesResponseType(typeof(ApiResponse<EnhancedChatResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Chat([FromBody] ChatRequestDto request)
        {
            if (request.Messages == null || !request.Messages.Any())
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Messages cannot be empty."));
            }

            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                
                // If conversation ID is provided, use continue conversation
                if (!string.IsNullOrEmpty(request.ConversationId))
                {
                    var lastMessage = request.Messages.LastOrDefault();
                    if (lastMessage?.Role == "user")
                    {
                        var response = await _chatService.ContinueConversationAsync(request.ConversationId, lastMessage.Content, userId);
                        
                        // Store pending plan suggestion
                        if (response.PlanSuggestion != null && response.ConfirmationId != null)
                        {
                            _pendingPlans[response.ConfirmationId] = response.PlanSuggestion;
                            CleanupExpiredConfirmations();
                        }
                        
                        return Ok(ApiResponse<EnhancedChatResponseDto>.SuccessResponse(response));
                    }
                }

                // Otherwise, use the legacy approach
                var legacyResponse = await _chatService.GetEnhancedChatCompletionAsync(request.Messages, userId);
                
                // Store pending plan suggestion
                if (legacyResponse.PlanSuggestion != null && legacyResponse.ConfirmationId != null)
                {
                    _pendingPlans[legacyResponse.ConfirmationId] = legacyResponse.PlanSuggestion;
                    CleanupExpiredConfirmations();
                }
                
                return Ok(ApiResponse<EnhancedChatResponseDto>.SuccessResponse(legacyResponse));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse($"Error processing chat request: {ex.Message}"));
            }
        }

        /// <summary>
        /// Simple chat without plan features
        /// </summary>
        /// <param name="request">Message history</param>
        /// <returns>Simple AI response</returns>
        [HttpPost("simple")]
        [SwaggerOperation(Summary = "Simple chat", OperationId = "SimpleChat")]
        [ProducesResponseType(typeof(ApiResponse<ChatMessageDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SimpleChat([FromBody] ChatRequestDto request)
        {
            if (request.Messages == null || !request.Messages.Any())
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Messages cannot be empty."));
            }

            try
            {
                var responseContent = await _chatService.GetChatCompletionAsync(request.Messages);
                var response = new ChatMessageDto 
                { 
                    Role = "assistant", 
                    Content = responseContent,
                    MessageId = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow
                };
                return Ok(ApiResponse<ChatMessageDto>.SuccessResponse(response));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse($"Error processing simple chat request: {ex.Message}"));
            }
        }

        /// <summary>
        /// Confirm plan creation
        /// </summary>
        /// <param name="confirmationId">Confirmation ID from AI response</param>
        /// <param name="confirmationDto">Confirmation details</param>
        /// <returns>Created plan info</returns>
        [HttpPost("confirm-plan/{confirmationId}")]
        [SwaggerOperation(Summary = "Confirm plan", OperationId = "ConfirmPlan")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConfirmPlan(string confirmationId, [FromBody] PlanConfirmationDto confirmationDto)
        {
            if (!_pendingPlans.TryGetValue(confirmationId, out var planSuggestion))
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Plan confirmation not found or expired."));
            }

            if (!confirmationDto.Confirmed)
            {
                _pendingPlans.TryRemove(confirmationId, out _);
                return Ok(ApiResponse<object>.SuccessResponse(null, "Plan creation cancelled."));
            }

            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Restrict confirmation to users who have active VIP or Group packages
                var userPackageService = HttpContext.RequestServices.GetRequiredService<IUserPackageService>();
                var packages = await userPackageService.GetPackageIdsByUserIdAsync(userId);
                var now = DateTime.UtcNow;
                var hasVipOrGroup = packages.Any(p => (p.IsActive ?? false)
                                                        && p.StartDate <= now && p.EndDate >= now
                                                        && (string.Equals(p.PackageName, "Gói VIP", StringComparison.OrdinalIgnoreCase)
                                                            || string.Equals(p.PackageName, "Gói Group", StringComparison.OrdinalIgnoreCase)));
                if (!hasVipOrGroup)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.ErrorResponse("Chatbot plan confirmation is available only for VIP or Group package users."));
                }

                var createdPlan = await _chatService.CreatePlanFromSuggestion(planSuggestion, userId);

                // Optional: link to group
                if (confirmationDto.GroupId.HasValue)
                {
                    var accessService = HttpContext.RequestServices.GetRequiredService<IPlanAccessService>();
                    await accessService.LinkPlanToGroupAsync(createdPlan.PlanId, confirmationDto.GroupId.Value, userId);
                }
                
                _pendingPlans.TryRemove(confirmationId, out _);
                
                return Ok(ApiResponse<object>.SuccessResponse(new { PlanId = createdPlan.PlanId, Message = "Plan created successfully!" }));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse($"Error creating plan: {ex.Message}"));
            }
        }

        /// <summary>
        /// Preview pending plan
        /// </summary>
        /// <param name="confirmationId">Confirmation ID</param>
        /// <returns>Plan suggestion details</returns>
        [HttpGet("pending-plan/{confirmationId}")]
        [SwaggerOperation(Summary = "Get pending plan", OperationId = "GetPendingPlan")]
        [ProducesResponseType(typeof(ApiResponse<PlanSuggestion>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public IActionResult GetPendingPlan(string confirmationId)
        {
            if (!_pendingPlans.TryGetValue(confirmationId, out var planSuggestion))
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Plan suggestion not found or expired."));
            }

            return Ok(ApiResponse<PlanSuggestion>.SuccessResponse(planSuggestion));
        }

        private void CleanupExpiredConfirmations()
        {
            var expiredKeys = _pendingPlans
                .Where(kvp => DateTime.UtcNow - kvp.Value.CreatedAt > TimeSpan.FromMinutes(30))
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                _pendingPlans.TryRemove(key, out _);
            }
        }
    }
} 