using AutoMapper;
using PlanyApp.Repository.Models;
using PlanyApp.Service.Dto.Items;
using PlanyApp.Service.Dto.Plan;
using PlanyApp.Service.Dto.ImageS3;

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
                .ForMember(dest => dest.PlanItems, opt => opt.MapFrom(src => src.PlanLists));
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
        }
    }
} 