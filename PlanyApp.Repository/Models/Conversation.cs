using System;
using System.Collections.Generic;

namespace PlanyApp.Repository.Models;

public partial class Conversation
{
    public Conversation()
    {
        ChatMessages = new HashSet<ChatMessage>();
    }

    public int ConversationId { get; set; }
    public int UserId { get; set; }
    public string? Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual ICollection<ChatMessage> ChatMessages { get; set; }
} 