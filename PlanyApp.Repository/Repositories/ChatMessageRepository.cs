using PlanyApp.Repository.Interfaces;
using PlanyApp.Repository.Models;
using System.Threading.Tasks;

namespace PlanyApp.Repository.Repositories
{
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly PlanyDBContext _context;

        public ChatMessageRepository(PlanyDBContext context)
        {
            _context = context;
        }

        public async Task<ChatMessage> AddAsync(ChatMessage message)
        {
            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }
    }
} 