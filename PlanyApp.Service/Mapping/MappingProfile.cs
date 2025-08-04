using AutoMapper;
using PlanyApp.Repository.Models;
using PlanyApp.Service.Dto;
using PlanyApp.Service.Dto.Auth;
using PlanyApp.Service.Dto.Challenge;
using PlanyApp.Service.Dto.Gift;
using PlanyApp.Service.Dto.Group;
using PlanyApp.Service.Dto.ImageS3;
using PlanyApp.Service.Dto.Items;
using PlanyApp.Service.Dto.Package;
using PlanyApp.Service.Dto.Plan;
using PlanyApp.Service.Dto.Province;
using PlanyApp.Service.Dto.UserPackage;
using Profile = AutoMapper.Profile;

namespace PlanyApp.Service.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Item mappings
            CreateMap<Item, ItemDto>();

            // Image mappings
            CreateMap<Image, ImageDto>();
            CreateMap<ImageS3, ImageS3Dto>();

            // Plan mappings
            CreateMap<Plan, PlanDto>()
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.PlanItems, opt => opt.MapFrom(src => src.PlanLists))
                .ForMember(dest => dest.Province, opt => opt.MapFrom(src => src.Province))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.HasValue ? src.StartDate.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.HasValue ? src.EndDate.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => 
                    src.Ratings.Any() ? src.Ratings.Average(r => r.Rate) : 0))
                .ForMember(dest => dest.RatingCount, opt => opt.MapFrom(src => src.Ratings.Count));
            CreateMap<CreatePlanRequestDto, Plan>()
                .ForMember(dest => dest.PlanLists, opt => opt.Ignore());
            CreateMap<UpdatePlanDto, Plan>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // PlanList mappings
            CreateMap<PlanList, PlanListDto>();
            CreateMap<PlanListItemDto, PlanList>()
                .ForMember(dest => dest.ItemId, opt => opt.Ignore());
            CreateMap<UpdatePlanListDto, PlanList>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UserPackage, ResponseListUserPackage>();
            CreateMap<Repository.Models.ImageS3, ImageS3Dto>();
            CreateMap<Repository.Models.Province, ProvinceDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Image));

            // Chat and Conversation mappings
            CreateMap<Conversation, ConversationHistoryDto>()
                .ForMember(dest => dest.ConversationId, opt => opt.MapFrom(src => src.ConversationId.ToString()))
                .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.ChatMessages.OrderBy(m => m.Timestamp)))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => src.LastUpdatedAt));
            
            CreateMap<ChatMessage, ChatMessageDto>()
                .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.MessageId.ToString()))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp));
            
            // Add reverse mapping for ChatMessage
            CreateMap<ChatMessageDto, ChatMessage>()
                .ForMember(dest => dest.MessageId, opt => opt.Ignore()) // Let database generate
                .ForMember(dest => dest.ConversationId, opt => opt.Ignore()) // Will be set separately
                .ForMember(dest => dest.Conversation, opt => opt.Ignore()); // Navigation property
        }
    }
} 