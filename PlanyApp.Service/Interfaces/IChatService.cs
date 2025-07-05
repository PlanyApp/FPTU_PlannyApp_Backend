using PlanyApp.Service.Dto;
using PlanyApp.Service.Dto.Plan;
using System.Threading.Tasks;

namespace PlanyApp.Service.Interfaces
{
    public interface IChatService
    {
        Task<string> GetChatCompletionAsync(List<ChatMessageDto> messages);
        Task<EnhancedChatResponseDto> GetEnhancedChatCompletionAsync(List<ChatMessageDto> messages, int userId);
        Task<PlanDto> CreatePlanFromSuggestion(PlanSuggestion suggestion, int userId);
        
        // New conversation-based methods
        Task<EnhancedChatResponseDto> StartConversationAsync(string initialMessage, int userId);
        Task<EnhancedChatResponseDto> ContinueConversationAsync(string conversationId, string message, int userId);
    }
} 