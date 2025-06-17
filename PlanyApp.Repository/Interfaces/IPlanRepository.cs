using System.Collections.Generic;
using System.Threading.Tasks;
using PlanyApp.Repository.Models;

namespace PlanyApp.Repository.Interfaces
{
    public interface IPlanRepository
    {
        Task<IEnumerable<Plan>> GetAllPlansAsync();
        Task<Plan> GetPlanByIdAsync(int planId);
        Task<IEnumerable<Plan>> GetPlansByOwnerIdAsync(int ownerId);
        Task<Plan> CreatePlanAsync(Plan plan);
        Task<Plan> UpdatePlanAsync(Plan plan);
        Task<bool> DeletePlanAsync(int planId);
        Task<bool> AddPlanItemAsync(PlanList planItem);
        Task<bool> UpdatePlanItemAsync(PlanList planItem);
        Task<bool> DeletePlanItemAsync(int planId, int itemId);
        Task<IEnumerable<PlanList>> GetPlanItemsAsync(int planId);
        Task<decimal> CalculateTotalCostAsync(int planId);
    }
} 