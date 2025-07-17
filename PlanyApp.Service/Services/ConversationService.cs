using AutoMapper;
using PlanyApp.Repository.UnitOfWork;
using PlanyApp.Service.Dto;
using PlanyApp.Service.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanyApp.Service.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        // In-memory storage: conversationId -> conversation data
        // private static readonly ConcurrentDictionary<string, ConversationData> _conversations = new();
        
        // User conversations mapping: userId -> list of conversationIds
        // private static readonly ConcurrentDictionary<int, List<string>> _userConversations = new();

        private class ConversationData
        {
            public string ConversationId { get; set; } = null!;
            public int UserId { get; set; }
            public List<ChatMessageDto> Messages { get; set; } = new();
            public DateTime CreatedAt { get; set; }
            public DateTime LastUpdated { get; set; }
        }

        public ConversationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> StartConversationAsync(int userId, string title)
        {
            var conversation = new Repository.Models.Conversation
            {
                UserId = userId,
                Title = title,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            };
            var newConversation = await _unitOfWork.ConversationRepository.AddAsync(conversation);
            return newConversation.ConversationId;
        }

        public async Task AddMessageAsync(int conversationId, ChatMessageDto message, int userId)
        {
            var conversation = await _unitOfWork.ConversationRepository.GetByIdAsync(conversationId);
            if (conversation == null || conversation.UserId != userId)
            {
                throw new UnauthorizedAccessException("Conversation not found or access denied.");
            }

            var chatMessage = new Repository.Models.ChatMessage
            {
                ConversationId = conversationId,
                Role = message.Role,
                Content = message.Content,
                Timestamp = message.Timestamp ?? DateTime.UtcNow,
            };
            
            await _unitOfWork.ChatMessageRepository.AddAsync(chatMessage);
            
            conversation.LastUpdatedAt = DateTime.UtcNow;
            _unitOfWork.ConversationRepository.Update(conversation);

            await _unitOfWork.SaveAsync();
        }

        public async Task<bool> DeleteConversationAsync(int conversationId, int userId)
        {
            var conversation = await _unitOfWork.ConversationRepository.GetByIdAsync(conversationId);
            if (conversation == null || conversation.UserId != userId)
            {
                return false;
            }

            await _unitOfWork.ConversationRepository.DeleteAsync(conversation);
            return true;
        }

        public async Task<ConversationHistoryDto?> GetConversationAsync(int conversationId, int userId)
        {
            var conversation = await _unitOfWork.ConversationRepository.GetByIdWithMessagesAsync(conversationId, userId);
            if (conversation == null)
            {
                return null;
            }
            
            return _mapper.Map<ConversationHistoryDto>(conversation);
        }

        public async Task<List<ConversationHistoryDto>> GetUserConversationsAsync(int userId)
        {
            var conversations = await _unitOfWork.ConversationRepository.GetByUserIdAsync(userId);
            return _mapper.Map<List<ConversationHistoryDto>>(conversations);
        }
    }
} 