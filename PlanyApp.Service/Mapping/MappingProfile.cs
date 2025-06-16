using AutoMapper;
using PlanyApp.Repository.Models;
using PlanyApp.Service.Dto.Items;
using PlanyApp.Service.Dto.Plan;

namespace PlanyApp.Service.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Item mappings
            CreateMap<Item, ItemDto>();

            // Plan mappings
            CreateMap<Plan, PlanDto>();
            CreateMap<CreatePlanDto, Plan>();
            CreateMap<UpdatePlanDto, Plan>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // PlanList mappings
            CreateMap<PlanList, PlanListDto>();
            CreateMap<CreatePlanListDto, PlanList>();
            CreateMap<UpdatePlanListDto, PlanList>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
} 