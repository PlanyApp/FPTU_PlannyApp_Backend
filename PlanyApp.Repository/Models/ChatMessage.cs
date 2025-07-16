using System;
using System.Collections.Generic;

namespace PlanyApp.Repository.Models;

public partial class ChatMessage
{
    public int MessageId { get; set; }
    public int ConversationId { get; set; }
    public string Role { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime Timestamp { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;
} 