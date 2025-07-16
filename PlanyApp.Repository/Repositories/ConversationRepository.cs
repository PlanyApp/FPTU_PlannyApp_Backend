using Microsoft.EntityFrameworkCore;
using PlanyApp.Repository.Interfaces;
using PlanyApp.Repository.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanyApp.Repository.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly PlanyDBContext _context;

        public ConversationRepository(PlanyDBContext context)
        {
            _context = context;
        }

        public async Task<Conversation> AddAsync(Conversation conversation)
        {
            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();
            return conversation;
        }

        public async Task DeleteAsync(Conversation conversation)
        {
            _context.Conversations.Remove(conversation);
            await _context.SaveChangesAsync();
        }

        public async Task<Conversation?> GetByIdWithMessagesAsync(int conversationId, int userId)
        {
            return await _context.Conversations
                .Include(c => c.ChatMessages)
                .FirstOrDefaultAsync(c => c.ConversationId == conversationId && c.UserId == userId);
        }

        public async Task<List<Conversation>> GetByUserIdAsync(int userId)
        {
            return await _context.Conversations
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.LastUpdatedAt)
                .ToListAsync();
        }

        public async Task<Conversation?> GetByIdAsync(int conversationId)
        {
            return await _context.Conversations.FindAsync(conversationId);
        }

        public void Update(Conversation conversation)
        {
            _context.Conversations.Update(conversation);
        }
    }
} 