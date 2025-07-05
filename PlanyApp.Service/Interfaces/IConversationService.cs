using PlanyApp.Service.Dto;
using System.Threading.Tasks;

namespace PlanyApp.Service.Interfaces
{
    public interface IConversationService
    {
        Task<string> StartConversationAsync(int userId);
        Task<ConversationHistoryDto?> GetConversationAsync(string conversationId, int userId);
        Task AddMessageAsync(string conversationId, ChatMessageDto message, int userId);
        Task<List<ConversationHistoryDto>> GetUserConversationsAsync(int userId);
        Task<bool> DeleteConversationAsync(string conversationId, int userId);
        Task CleanupOldConversationsAsync();
    }
} 