using PlanyApp.Repository.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanyApp.Repository.Interfaces
{
    public interface IConversationRepository
    {
        Task<Conversation> AddAsync(Conversation conversation);
        Task<Conversation?> GetByIdWithMessagesAsync(int conversationId, int userId);
        Task<List<Conversation>> GetByUserIdAsync(int userId);
        Task DeleteAsync(Conversation conversation);
        Task<Conversation?> GetByIdAsync(int conversationId);
        void Update(Conversation conversation);
    }
} 