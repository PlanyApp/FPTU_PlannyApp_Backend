using PlanyApp.Service.Dto;
using System.Threading.Tasks;

namespace PlanyApp.Service.Interfaces
{
    public interface IConversationService
    {
        Task<int> StartConversationAsync(int userId, string title);
        Task<ConversationHistoryDto?> GetConversationAsync(int conversationId, int userId);
        Task AddMessageAsync(int conversationId, ChatMessageDto message, int userId);
        Task<List<ConversationHistoryDto>> GetUserConversationsAsync(int userId);
        Task<bool> DeleteConversationAsync(int conversationId, int userId);
    }
} 