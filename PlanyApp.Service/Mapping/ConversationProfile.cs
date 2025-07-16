using AutoMapper;
using PlanyApp.Repository.Models;
using PlanyApp.Service.Dto;

namespace PlanyApp.Service.Mapping
{
    public class ConversationProfile : Profile
    {
        public ConversationProfile()
        {
            CreateMap<Conversation, ConversationHistoryDto>()
                .ForMember(dest => dest.ConversationId, opt => opt.MapFrom(src => src.ConversationId.ToString()));
            CreateMap<ChatMessage, ChatMessageDto>().ReverseMap();
        }
    }
} 