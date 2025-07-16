using PlanyApp.Repository.Models;
using System.Threading.Tasks;

namespace PlanyApp.Repository.Interfaces
{
    public interface IChatMessageRepository
    {
        Task<ChatMessage> AddAsync(ChatMessage message);
    }
} 