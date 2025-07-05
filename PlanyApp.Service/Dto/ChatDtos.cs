using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PlanyApp.Service.Dto
{
    /// <summary>
    /// Chat request with message history
    /// </summary>
    /// <example>
    /// {
    ///   "messages": [
    ///     {
    ///       "role": "user",
    ///       "content": "I want to plan a 5-day trip to Tokyo in March with a $2000 budget"
    ///     }
    ///   ],
    ///   "conversationId": "conv_abc123def456"
    /// }
    /// </example>
    public class ChatRequestDto
    {
        /// <summary>
        /// List of chat messages
        /// </summary>
        /// <example>
        /// [
        ///   {
        ///     "role": "user",
        ///     "content": "Plan a 3-day trip to Kyoto"
        ///   },
        ///   {
        ///     "role": "assistant", 
        ///     "content": "I'd be happy to help! What's your budget and travel dates?"
        ///   },
        ///   {
        ///     "role": "user",
        ///     "content": "$1500 total, traveling in April"
        ///   }
        /// ]
        /// </example>
        [Required]
        public required List<ChatMessageDto> Messages { get; set; }
        
        /// <summary>
        /// Optional conversation ID
        /// </summary>
        /// <example>conv_abc123def456</example>
        public string? ConversationId { get; set; }
    }

    /// <summary>
    /// Individual chat message
    /// </summary>
    /// <example>
    /// {
    ///   "messageId": "msg_789xyz123",
    ///   "role": "user",
    ///   "content": "I want to plan a romantic weekend in Paris",
    ///   "timestamp": "2024-01-15T10:30:00Z"
    /// }
    /// </example>
    public class ChatMessageDto
    {
        /// <summary>
        /// Message ID (auto-generated if empty)
        /// </summary>
        /// <example>msg_789xyz123</example>
        public string? MessageId { get; set; }
        
        /// <summary>
        /// Message role: "user" or "assistant"
        /// </summary>
        /// <example>user</example>
        [Required]
        public required string Role { get; set; }
        
        /// <summary>
        /// Message content
        /// </summary>
        /// <example>I want to plan a romantic weekend in Paris for under $1000</example>
        [Required]
        public required string Content { get; set; }
        
        /// <summary>
        /// Message timestamp
        /// </summary>
        /// <example>2024-01-15T10:30:00Z</example>
        public DateTime? Timestamp { get; set; }
    }

    /// <summary>
    /// Start new conversation request
    /// </summary>
    /// <example>
    /// {
    ///   "initialMessage": "I want to plan a 7-day family vacation to Thailand for 4 people in December"
    /// }
    /// </example>
    public class StartConversationDto
    {
        /// <summary>
        /// Initial message to AI
        /// </summary>
        /// <example>I want to plan a 7-day family vacation to Thailand for 4 people in December</example>
        [Required]
        [StringLength(2000, MinimumLength = 1)]
        public required string InitialMessage { get; set; }
    }

    /// <summary>
    /// Continue conversation request
    /// </summary>
    /// <example>
    /// {
    ///   "conversationId": "conv_abc123def456",
    ///   "message": "Make it more budget-friendly, around $1200 total"
    /// }
    /// </example>
    public class ContinueConversationDto
    {
        /// <summary>
        /// Conversation ID
        /// </summary>
        /// <example>conv_abc123def456</example>
        [Required]
        public required string ConversationId { get; set; }
        
        /// <summary>
        /// New message
        /// </summary>
        /// <example>Make it more budget-friendly, around $1200 total</example>
        [Required]
        [StringLength(2000, MinimumLength = 1)]
        public required string Message { get; set; }
    }

    /// <summary>
    /// AI response with plan suggestions
    /// </summary>
    /// <example>
    /// {
    ///   "role": "assistant",
    ///   "content": "I've created a fantastic 5-day Tokyo itinerary for you! Here's what I suggest:\n\n🎯 **PLAN_SUGGESTION_START**\n{\"name\":\"Tokyo Adventure\",\"startDate\":\"2024-03-15\",\"endDate\":\"2024-03-20\",\"items\":[{\"name\":\"Sensoji Temple\",\"itemType\":\"place\",\"dayNumber\":1,\"itemNo\":1,\"startTime\":\"09:00\",\"endTime\":\"11:00\",\"notes\":\"Historic temple in Asakusa\",\"price\":0}],\"estimatedTotalCost\":1850}\n🎯 **PLAN_SUGGESTION_END**\n\nWould you like me to create this plan in your account?",
    ///   "conversationId": "conv_abc123def456",
    ///   "messageId": "msg_789xyz123",
    ///   "timestamp": "2024-01-15T10:30:00Z",
    ///   "planSuggestion": {
    ///     "name": "Tokyo Adventure",
    ///     "startDate": "2024-03-15",
    ///     "endDate": "2024-03-20",
    ///     "items": [
    ///       {
    ///         "name": "Sensoji Temple",
    ///         "itemType": "place",
    ///         "dayNumber": 1,
    ///         "itemNo": 1,
    ///         "startTime": "09:00",
    ///         "endTime": "11:00",
    ///         "notes": "Historic temple in Asakusa",
    ///         "price": 0
    ///       }
    ///     ],
    ///     "estimatedTotalCost": 1850
    ///   },
    ///   "requiresConfirmation": true,
    ///   "confirmationId": "confirm_xyz789abc123"
    /// }
    /// </example>
    public class EnhancedChatResponseDto
    {
        /// <summary>
        /// Response role (always "assistant")
        /// </summary>
        /// <example>assistant</example>
        public string Role { get; set; } = "assistant";
        
        /// <summary>
        /// AI response content
        /// </summary>
        /// <example>I've created a fantastic 5-day Tokyo itinerary for you! Here's what I suggest: [plan details]. Would you like me to create this plan in your account?</example>
        public string Content { get; set; } = null!;
        
        /// <summary>
        /// Conversation ID
        /// </summary>
        /// <example>conv_abc123def456</example>
        public string ConversationId { get; set; } = null!;
        
        /// <summary>
        /// Message ID
        /// </summary>
        /// <example>msg_789xyz123</example>
        public string MessageId { get; set; } = null!;
        
        /// <summary>
        /// Response timestamp
        /// </summary>
        /// <example>2024-01-15T10:30:00Z</example>
        public DateTime Timestamp { get; set; }
        
        /// <summary>
        /// Plan suggestion if generated
        /// </summary>
        public PlanSuggestion? PlanSuggestion { get; set; }
        
        /// <summary>
        /// Whether confirmation is required
        /// </summary>
        /// <example>true</example>
        public bool RequiresConfirmation { get; set; }
        
        /// <summary>
        /// Confirmation ID for plan creation
        /// </summary>
        /// <example>confirm_xyz789abc123</example>
        public string? ConfirmationId { get; set; }
        
        /// <summary>
        /// Full conversation history
        /// </summary>
        public List<ChatMessageDto> ConversationHistory { get; set; } = new();
    }

    /// <summary>
    /// Conversation history
    /// </summary>
    /// <example>
    /// {
    ///   "conversationId": "conv_abc123def456",
    ///   "messages": [
    ///     {
    ///       "messageId": "msg_user_001",
    ///       "role": "user",
    ///       "content": "Plan a trip to Tokyo",
    ///       "timestamp": "2024-01-15T10:00:00Z"
    ///     },
    ///     {
    ///       "messageId": "msg_ai_001",
    ///       "role": "assistant",
    ///       "content": "I'd love to help you plan a Tokyo trip! What's your budget and travel dates?",
    ///       "timestamp": "2024-01-15T10:00:30Z"
    ///     }
    ///   ],
    ///   "createdAt": "2024-01-15T10:00:00Z",
    ///   "lastUpdated": "2024-01-15T10:00:30Z"
    /// }
    /// </example>
    public class ConversationHistoryDto
    {
        /// <summary>
        /// Conversation ID
        /// </summary>
        /// <example>conv_abc123def456</example>
        public string ConversationId { get; set; } = null!;
        
        /// <summary>
        /// All messages in order
        /// </summary>
        public List<ChatMessageDto> Messages { get; set; } = new();
        
        /// <summary>
        /// Creation date
        /// </summary>
        /// <example>2024-01-15T10:00:00Z</example>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Last update date
        /// </summary>
        /// <example>2024-01-15T10:00:30Z</example>
        public DateTime LastUpdated { get; set; }
    }

    /// <summary>
    /// AI-generated plan suggestion
    /// </summary>
    /// <example>
    /// {
    ///   "name": "Amazing Tokyo Adventure",
    ///   "startDate": "2024-03-15",
    ///   "endDate": "2024-03-20",
    ///   "items": [
    ///     {
    ///       "name": "Sensoji Temple",
    ///       "itemType": "place",
    ///       "dayNumber": 1,
    ///       "itemNo": 1,
    ///       "startTime": "09:00",
    ///       "endTime": "11:00",
    ///       "notes": "Historic temple in Asakusa district",
    ///       "price": 0,
    ///       "existingItemId": null
    ///     },
    ///     {
    ///       "name": "Park Hyatt Tokyo",
    ///       "itemType": "hotel",
    ///       "dayNumber": 1,
    ///       "itemNo": 2,
    ///       "startTime": "15:00",
    ///       "endTime": "11:00",
    ///       "notes": "Luxury hotel in Shinjuku",
    ///       "price": 400,
    ///       "existingItemId": 123
    ///     },
    ///     {
    ///       "name": "JR Yamanote Line",
    ///       "itemType": "transportation",
    ///       "dayNumber": 2,
    ///       "itemNo": 1,
    ///       "startTime": "08:30",
    ///       "endTime": "09:00",
    ///       "notes": "Train from Shinjuku to Harajuku",
    ///       "price": 3,
    ///       "existingItemId": null
    ///     }
    ///   ],
    ///   "estimatedTotalCost": 1850,
    ///   "createdAt": "2024-01-15T10:30:00Z"
    /// }
    /// </example>
    public class PlanSuggestion
    {
        /// <summary>
        /// Plan name
        /// </summary>
        /// <example>Amazing Tokyo Adventure</example>
        public string Name { get; set; } = null!;
        
        /// <summary>
        /// Trip start date
        /// </summary>
        /// <example>2024-03-15</example>
        public DateTime? StartDate { get; set; }
        
        /// <summary>
        /// Trip end date
        /// </summary>
        /// <example>2024-03-20</example>
        public DateTime? EndDate { get; set; }
        
        /// <summary>
        /// Plan items (activities, hotels, transport)
        /// </summary>
        public List<AISuggestedItem> Items { get; set; } = new();
        
        /// <summary>
        /// Total estimated cost in USD
        /// </summary>
        /// <example>1850</example>
        public decimal EstimatedTotalCost { get; set; }
        
        /// <summary>
        /// Suggestion creation time
        /// </summary>
        /// <example>2024-01-15T10:30:00Z</example>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Individual plan item suggestion
    /// </summary>
    /// <example>
    /// {
    ///   "name": "Sensoji Temple",
    ///   "itemType": "place",
    ///   "dayNumber": 1,
    ///   "itemNo": 1,
    ///   "startTime": "09:00",
    ///   "endTime": "11:00",
    ///   "notes": "Historic temple in Asakusa district - don't miss the traditional shopping street",
    ///   "price": 0,
    ///   "existingItemId": 456
    /// }
    /// </example>
    public class AISuggestedItem
    {
        /// <summary>
        /// Item name
        /// </summary>
        /// <example>Sensoji Temple</example>
        public string Name { get; set; } = null!;
        
        /// <summary>
        /// Item type: "place", "hotel", or "transportation"
        /// </summary>
        /// <example>place</example>
        [Required]
        public string ItemType { get; set; } = null!;
        
        /// <summary>
        /// Day number (1-based)
        /// </summary>
        /// <example>1</example>
        [Range(1, 365)]
        public int DayNumber { get; set; }
        
        /// <summary>
        /// Item order within day (1-based)
        /// </summary>
        /// <example>1</example>
        [Range(1, 50)]
        public int ItemNo { get; set; }
        
        /// <summary>
        /// Start time (HH:mm)
        /// </summary>
        /// <example>09:00</example>
        public string? StartTime { get; set; }
        
        /// <summary>
        /// End time (HH:mm)
        /// </summary>
        /// <example>11:00</example>
        public string? EndTime { get; set; }
        
        /// <summary>
        /// Additional notes
        /// </summary>
        /// <example>Historic temple in Asakusa district - don't miss the traditional shopping street</example>
        public string? Notes { get; set; }
        
        /// <summary>
        /// Estimated cost in USD
        /// </summary>
        /// <example>0</example>
        [Range(0, 10000)]
        public decimal? Price { get; set; }
        
        /// <summary>
        /// Existing venue ID if matched
        /// </summary>
        /// <example>456</example>
        public int? ExistingItemId { get; set; }
    }

    /// <summary>
    /// Plan confirmation request
    /// </summary>
    /// <example>
    /// {
    ///   "confirmed": true,
    ///   "planTitle": "My Amazing Tokyo Adventure"
    /// }
    /// </example>
    public class PlanConfirmationDto
    {
        /// <summary>
        /// Confirm (true) or reject (false) plan
        /// </summary>
        /// <example>true</example>
        [Required]
        public bool Confirmed { get; set; }
        
        /// <summary>
        /// Custom plan title (optional)
        /// </summary>
        /// <example>My Amazing Tokyo Adventure</example>
        [StringLength(200)]
        public string? PlanTitle { get; set; }
    }
} 