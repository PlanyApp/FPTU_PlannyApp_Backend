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

            // Add the new profile
            CreateMap<Conversation, ConversationHistoryDto>()
                .ForMember(dest => dest.ConversationId, opt => opt.MapFrom(src => src.ConversationId.ToString()));
            CreateMap<ChatMessage, ChatMessageDto>();
        }
    }
} 