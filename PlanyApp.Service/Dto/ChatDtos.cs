namespace PlanyApp.Service.Dto
{
    public class ChatRequestDto
    {
        public required List<ChatMessageDto> Messages { get; set; }
    }

    public class ChatMessageDto
    {
        public required string Role { get; set; }
        public required string Content { get; set; }
    }
} 