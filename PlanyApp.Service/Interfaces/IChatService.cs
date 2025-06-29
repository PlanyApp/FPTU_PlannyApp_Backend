using PlanyApp.Service.Dto;

namespace PlanyApp.Service.Interfaces
{
    public interface IChatService
    {
        Task<string> GetChatCompletionAsync(List<ChatMessageDto> messages);
    }
} 