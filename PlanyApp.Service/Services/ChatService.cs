using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using PlanyApp.Service.Dto;
using PlanyApp.Service.Dto.Plan;
using PlanyApp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace PlanyApp.Service.Services
{
    public class ChatService : IChatService
    {
        private readonly OpenAIClient _client;
        private readonly string _modelName;
        private readonly IConfiguration _configuration;
        private readonly IPlanService _planService;
        private readonly IItemService _itemService;
        private readonly IConversationService _conversationService;

        private const string EnhancedSystemPrompt = @"[SYSTEM PROMPT - ENHANCED FOR PLAN CREATION]

You are Plany, a helpful AI assistant for the Plany travel application.
Your role is to assist users with their travel planning questions and CREATE DETAILED TRAVEL PLANS.
**You must respond in the same language the user uses.**

**CORE CAPABILITIES:**
1. Answer travel questions
2. **CREATE DETAILED TRAVEL PLANS** when users request trip planning

**PLAN CREATION PROTOCOL:**
When a user asks to create a trip/plan/itinerary, you MUST:

1. **Gather Information**: Ask for missing details (destination, dates, budget, interests)
2. **Generate Structured Plan**: Create a detailed itinerary
3. **Request Confirmation**: Ask if they want to save this as a real plan

**PLAN OUTPUT FORMAT:**
When suggesting a plan, end your response with:
```
🎯 **PLAN_SUGGESTION_START**
{
  ""name"": ""Trip Name"",
  ""startDate"": ""2025-07-15T00:00:00Z"",
  ""endDate"": ""2025-07-18T00:00:00Z"",
  ""estimatedTotalCost"": 500.0,
  ""items"": [
    {
      ""name"": ""Activity Name"",
      ""itemType"": ""place"",
      ""dayNumber"": 1,
      ""itemNo"": 1,
      ""startTime"": ""09:00:00"",
      ""endTime"": ""11:00:00"",
      ""notes"": ""Description"",
      ""price"": 50.0
    }
  ]
}
**PLAN_SUGGESTION_END** 🎯

Would you like me to create this plan for you in the app?
```

**IMPORTANT RULES:**
- **ITEM TYPES:** Only use ""place"", ""hotel"", ""transportation""
- **PRICING:** Provide realistic estimates in USD
- **TIMING:** Use 24-hour format (HH:mm:ss)
- **DAY NUMBERS:** Start from 1, increment for each day
- **ITEM NUMBERS:** Start from 1 for each day, increment within the same day

**2. PERSONA: Plany**
- **Name**: Plany.
- **Personality**: Enthusiastic, friendly, knowledgeable, and trustworthy.
- **Language**: Always respond in the same language the user uses.

**3. STRICT BOUNDARIES**
- **SCOPE: TRAVEL ONLY.**
- When faced with non-travel topics, politely decline and pivot back to travel.

[END OF SYSTEM PROMPT]";

        public ChatService(IConfiguration configuration, IPlanService planService, IItemService itemService, IConversationService conversationService)
        {
            _configuration = configuration;
            _planService = planService;
            _itemService = itemService;
            _conversationService = conversationService;

            var apiKey = _configuration["OpenAI:ApiKey"];
            var baseUrl = _configuration["OpenAI:BaseUrl"];
            _modelName = _configuration["OpenAI:ModelName"] ?? "gpt-4o-all";

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("OpenAI API Key or Base URL is not configured in environment variables.");
            }

            var apiDomain = new Uri(baseUrl).Host;
            var settings = new OpenAIClientSettings(domain: apiDomain);
            var auth = new OpenAIAuthentication(apiKey);
            _client = new OpenAIClient(auth, settings);
        }

        public async Task<string> GetChatCompletionAsync(List<ChatMessageDto> messages)
        {
            var requestMessages = new List<Message>
            {
                new(Role.System, EnhancedSystemPrompt)
            };

            foreach (var message in messages)
            {
                requestMessages.Add(new Message(
                    message.Role.Equals("user", StringComparison.OrdinalIgnoreCase) ? Role.User : Role.Assistant,
                    message.Content));
            }

            var chatRequest = new ChatRequest(requestMessages, model: _modelName, temperature: 0.7, maxTokens: 1200);

            try
            {
                var result = await _client.ChatEndpoint.GetCompletionAsync(chatRequest);
                return result.FirstChoice.Message;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "There was an error processing your request. Please try again later.";
            }
        }

        public async Task<EnhancedChatResponseDto> GetEnhancedChatCompletionAsync(List<ChatMessageDto> messages, int userId)
        {
            try
            {
                // Call AI with enhanced prompt
                var aiResponse = await GetChatCompletionAsync(messages);
                
                // Parse for plan suggestions
                var planSuggestion = ExtractPlanSuggestion(aiResponse);
                
                // Clean the response content (remove the JSON block)
                var cleanContent = CleanResponseContent(aiResponse);
                
                return new EnhancedChatResponseDto
                {
                    Content = cleanContent,
                    ConversationId = "", // Will be set by caller
                    MessageId = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow,
                    PlanSuggestion = planSuggestion,
                    RequiresConfirmation = planSuggestion != null,
                    ConfirmationId = planSuggestion != null ? Guid.NewGuid().ToString() : null,
                    ConversationHistory = messages
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in enhanced chat completion: {ex}");
                return new EnhancedChatResponseDto
                {
                    Content = "I apologize, but I encountered an error while processing your request. Please try again.",
                    ConversationId = "",
                    MessageId = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow,
                    RequiresConfirmation = false
                };
            }
        }

        public async Task<EnhancedChatResponseDto> StartConversationAsync(string initialMessage, int userId)
        {
            // Create new conversation
            var conversationId = await _conversationService.StartConversationAsync(userId);
            
            // Add user's initial message to conversation
            var userMessage = new ChatMessageDto
            {
                Role = "user",
                Content = initialMessage,
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow
            };
            await _conversationService.AddMessageAsync(conversationId, userMessage, userId);

            // Get AI response
            var messages = new List<ChatMessageDto> { userMessage };
            var response = await GetEnhancedChatCompletionAsync(messages, userId);
            response.ConversationId = conversationId;

            // Add AI response to conversation
            var aiMessage = new ChatMessageDto
            {
                Role = "assistant",
                Content = response.Content,
                MessageId = response.MessageId,
                Timestamp = response.Timestamp
            };
            await _conversationService.AddMessageAsync(conversationId, aiMessage, userId);

            // Get full conversation history
            var conversation = await _conversationService.GetConversationAsync(conversationId, userId);
            response.ConversationHistory = conversation?.Messages ?? new List<ChatMessageDto>();

            return response;
        }

        public async Task<EnhancedChatResponseDto> ContinueConversationAsync(string conversationId, string message, int userId)
        {
            // Get existing conversation
            var conversation = await _conversationService.GetConversationAsync(conversationId, userId);
            if (conversation == null)
            {
                throw new ArgumentException("Conversation not found or access denied.");
            }

            // Add user's new message to conversation
            var userMessage = new ChatMessageDto
            {
                Role = "user",
                Content = message,
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow
            };
            await _conversationService.AddMessageAsync(conversationId, userMessage, userId);

            // Prepare messages for AI (include conversation history)
            var allMessages = new List<ChatMessageDto>(conversation.Messages) { userMessage };

            // Get AI response
            var response = await GetEnhancedChatCompletionAsync(allMessages, userId);
            response.ConversationId = conversationId;

            // Add AI response to conversation
            var aiMessage = new ChatMessageDto
            {
                Role = "assistant",
                Content = response.Content,
                MessageId = response.MessageId,
                Timestamp = response.Timestamp
            };
            await _conversationService.AddMessageAsync(conversationId, aiMessage, userId);

            // Get updated conversation history
            var updatedConversation = await _conversationService.GetConversationAsync(conversationId, userId);
            response.ConversationHistory = updatedConversation?.Messages ?? new List<ChatMessageDto>();

            return response;
        }

        public async Task<PlanDto> CreatePlanFromSuggestion(PlanSuggestion suggestion, int userId)
        {
            var createPlanDto = await ConvertSuggestionToPlanRequest(suggestion);
            return await _planService.CreatePlanAsync(createPlanDto, userId);
        }

        private PlanSuggestion? ExtractPlanSuggestion(string aiResponse)
        {
            try
            {
                // Look for the plan suggestion pattern
                var pattern = @"🎯 \*\*PLAN_SUGGESTION_START\*\*\s*```(?:json)?\s*(\{.*?\})\s*```\s*\*\*PLAN_SUGGESTION_END\*\* 🎯";
                var match = Regex.Match(aiResponse, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                
                if (!match.Success)
                {
                    // Try alternative pattern without code blocks
                    pattern = @"🎯 \*\*PLAN_SUGGESTION_START\*\*\s*(\{.*?\})\s*\*\*PLAN_SUGGESTION_END\*\* 🎯";
                    match = Regex.Match(aiResponse, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                }

                if (match.Success)
                {
                    var jsonString = match.Groups[1].Value.Trim();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    
                    return JsonSerializer.Deserialize<PlanSuggestion>(jsonString, options);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing plan suggestion: {ex.Message}");
            }

            return null;
        }

        private string CleanResponseContent(string aiResponse)
        {
            // Remove the plan suggestion block from the response
            var pattern = @"🎯 \*\*PLAN_SUGGESTION_START\*\*.*?\*\*PLAN_SUGGESTION_END\*\* 🎯";
            var cleaned = Regex.Replace(aiResponse, pattern, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            
            return cleaned.Trim();
        }

        private async Task<CreatePlanRequestDto> ConvertSuggestionToPlanRequest(PlanSuggestion suggestion)
        {
            var items = new List<PlanListItemDto>();
            
            foreach (var aiItem in suggestion.Items)
            {
                // Try to match with existing items first
                var existingItem = await TryMatchExistingItem(aiItem);
                
                items.Add(new PlanListItemDto
                {
                    ItemId = existingItem?.ItemId,
                    Name = aiItem.Name,
                    ItemType = aiItem.ItemType,
                    DayNumber = aiItem.DayNumber,
                    ItemNo = aiItem.ItemNo,
                    StartTime = TimeOnly.TryParse(aiItem.StartTime, out var start) ? start : null,
                    EndTime = TimeOnly.TryParse(aiItem.EndTime, out var end) ? end : null,
                    Notes = aiItem.Notes,
                    Price = aiItem.Price
                });
            }

            return new CreatePlanRequestDto
            {
                Name = suggestion.Name,
                StartDate = suggestion.StartDate,
                EndDate = suggestion.EndDate,
                IsPublic = false, // Default to private
                Items = items
            };
        }

        private async Task<dynamic?> TryMatchExistingItem(AISuggestedItem aiItem)
        {
            try
            {
                // Simple name-based matching logic
                // You can enhance this with more sophisticated matching
                switch (aiItem.ItemType.ToLower())
                {
                    case "hotel":
                        var hotels = await _itemService.SearchHotelsByNameAsync(aiItem.Name);
                        return hotels.FirstOrDefault();
                    
                    case "place":
                        var places = await _itemService.SearchPlacesByNameAsync(aiItem.Name);
                        return places.FirstOrDefault();
                    
                    case "transportation":
                        var transports = await _itemService.SearchTransportationsByNameAsync(aiItem.Name);
                        return transports.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error matching existing item: {ex.Message}");
            }

            return null;
        }
    }
} 