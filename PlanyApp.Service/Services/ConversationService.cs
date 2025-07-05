using PlanyApp.Service.Dto;
using PlanyApp.Service.Interfaces;
using System.Collections.Concurrent;

namespace PlanyApp.Service.Services
{
    public class ConversationService : IConversationService
    {
        // In-memory storage: conversationId -> conversation data
        private static readonly ConcurrentDictionary<string, ConversationData> _conversations = new();
        
        // User conversations mapping: userId -> list of conversationIds
        private static readonly ConcurrentDictionary<int, List<string>> _userConversations = new();

        private class ConversationData
        {
            public string ConversationId { get; set; } = null!;
            public int UserId { get; set; }
            public List<ChatMessageDto> Messages { get; set; } = new();
            public DateTime CreatedAt { get; set; }
            public DateTime LastUpdated { get; set; }
        }

        public Task<string> StartConversationAsync(int userId)
        {
            var conversationId = Guid.NewGuid().ToString();
            var conversationData = new ConversationData
            {
                ConversationId = conversationId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            _conversations[conversationId] = conversationData;

            // Add to user's conversation list
            _userConversations.AddOrUpdate(userId, 
                new List<string> { conversationId },
                (key, existingList) => 
                {
                    existingList.Add(conversationId);
                    return existingList;
                });

            return Task.FromResult(conversationId);
        }

        public Task<ConversationHistoryDto?> GetConversationAsync(string conversationId, int userId)
        {
            if (!_conversations.TryGetValue(conversationId, out var conversation) || 
                conversation.UserId != userId)
            {
                return Task.FromResult<ConversationHistoryDto?>(null);
            }

            var historyDto = new ConversationHistoryDto
            {
                ConversationId = conversation.ConversationId,
                Messages = new List<ChatMessageDto>(conversation.Messages),
                CreatedAt = conversation.CreatedAt,
                LastUpdated = conversation.LastUpdated
            };

            return Task.FromResult<ConversationHistoryDto?>(historyDto);
        }

        public Task AddMessageAsync(string conversationId, ChatMessageDto message, int userId)
        {
            if (!_conversations.TryGetValue(conversationId, out var conversation) || 
                conversation.UserId != userId)
            {
                throw new UnauthorizedAccessException("Conversation not found or access denied.");
            }

            // Ensure message has ID and timestamp
            if (string.IsNullOrEmpty(message.MessageId))
            {
                message.MessageId = Guid.NewGuid().ToString();
            }
            if (!message.Timestamp.HasValue)
            {
                message.Timestamp = DateTime.UtcNow;
            }

            conversation.Messages.Add(message);
            conversation.LastUpdated = DateTime.UtcNow;

            return Task.CompletedTask;
        }

        public Task<List<ConversationHistoryDto>> GetUserConversationsAsync(int userId)
        {
            var result = new List<ConversationHistoryDto>();

            if (_userConversations.TryGetValue(userId, out var conversationIds))
            {
                foreach (var conversationId in conversationIds)
                {
                    if (_conversations.TryGetValue(conversationId, out var conversation))
                    {
                        result.Add(new ConversationHistoryDto
                        {
                            ConversationId = conversation.ConversationId,
                            Messages = new List<ChatMessageDto>(conversation.Messages),
                            CreatedAt = conversation.CreatedAt,
                            LastUpdated = conversation.LastUpdated
                        });
                    }
                }
            }

            // Sort by last updated (most recent first)
            result.Sort((a, b) => b.LastUpdated.CompareTo(a.LastUpdated));

            return Task.FromResult(result);
        }

        public Task<bool> DeleteConversationAsync(string conversationId, int userId)
        {
            if (!_conversations.TryGetValue(conversationId, out var conversation) || 
                conversation.UserId != userId)
            {
                return Task.FromResult(false);
            }

            // Remove from global conversations
            _conversations.TryRemove(conversationId, out _);

            // Remove from user's conversation list
            if (_userConversations.TryGetValue(userId, out var userConversations))
            {
                userConversations.Remove(conversationId);
            }

            return Task.FromResult(true);
        }

        public Task CleanupOldConversationsAsync()
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-30); // Keep conversations for 30 days
            var conversationsToRemove = new List<string>();

            foreach (var kvp in _conversations)
            {
                if (kvp.Value.LastUpdated < cutoffDate)
                {
                    conversationsToRemove.Add(kvp.Key);
                }
            }

            foreach (var conversationId in conversationsToRemove)
            {
                if (_conversations.TryRemove(conversationId, out var conversation))
                {
                    // Also remove from user's conversation list
                    if (_userConversations.TryGetValue(conversation.UserId, out var userConversations))
                    {
                        userConversations.Remove(conversationId);
                    }
                }
            }

            Console.WriteLine($"Cleaned up {conversationsToRemove.Count} old conversations.");
            return Task.CompletedTask;
        }
    }
} 