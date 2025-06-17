using System.Collections.Generic;
using System.Threading.Tasks;
using PlanyApp.Service.Dto.Plan;

namespace PlanyApp.Service.Interfaces
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanDto>> GetAllPlansAsync();
        Task<PlanDto> GetPlanByIdAsync(int planId);
        Task<IEnumerable<PlanDto>> GetPlansByOwnerIdAsync(int ownerId);
        Task<PlanDto> CreatePlanAsync(CreatePlanDto createPlanDto);
        Task<PlanDto> UpdatePlanAsync(int planId, UpdatePlanDto updatePlanDto);
        Task<bool> DeletePlanAsync(int planId);
        Task<PlanDto> AddPlanItemAsync(int planId, CreatePlanListDto createPlanListDto);
        Task<PlanDto> UpdatePlanItemAsync(int planId, int planListId, UpdatePlanListDto updatePlanListDto);
        Task<bool> DeletePlanItemAsync(int planId, int itemId);
        Task<IEnumerable<PlanListDto>> GetPlanItemsAsync(int planId);
        Task<decimal> GetPlanTotalCostAsync(int planId);
    }
} 