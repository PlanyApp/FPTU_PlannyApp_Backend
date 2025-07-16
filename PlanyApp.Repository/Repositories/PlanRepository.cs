using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using PlanyApp.Repository.Interfaces;
using PlanyApp.Repository.Models;

namespace PlanyApp.Repository.Repositories
{
    public class PlanRepository : IPlanRepository
    {
        private readonly PlanyDBContext _context;

        public PlanRepository(PlanyDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Plan>> GetAllPlansAsync()
        {
            return await _context.Plans
                .Include(p => p.PlanLists)
                    .ThenInclude(pl => pl.Item)
                .Include(p => p.Province)
                .Include(p => p.Ratings)
                .ToListAsync();
        }

        public async Task<Plan> GetPlanByIdAsync(int planId)
        {
            return await _context.Plans
                .Include(p => p.PlanLists)
                    .ThenInclude(pl => pl.Item)
                .Include(p => p.Province)
                .Include(p => p.Ratings)
                .FirstOrDefaultAsync(p => p.PlanId == planId);
        }

        public async Task<IEnumerable<Plan>> GetPlansByOwnerIdAsync(int ownerId)
        {
            return await _context.Plans
                .Include(p => p.PlanLists)
                    .ThenInclude(pl => pl.Item)
                .Include(p => p.Province)
                .Include(p => p.Ratings)
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task<Plan> CreatePlanAsync(Plan plan)
        {
            plan.CreatedAt = DateTime.UtcNow;
            plan.UpdatedAt = DateTime.UtcNow;
            //plan.DateCreated = DateTime.UtcNow;
            //plan.Status = "Draft";
            
            await _context.Plans.AddAsync(plan);
            await _context.SaveChangesAsync();
            return plan;
        }

        public async Task<Plan> UpdatePlanAsync(Plan plan)
        {
            var existingPlan = await _context.Plans.FindAsync(plan.PlanId);
            if (existingPlan == null) return null;

            existingPlan.Name = plan.Name;
           // existingPlan.Status = plan.Status;
            existingPlan.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingPlan;
        }

        public async Task<bool> DeletePlanAsync(int planId)
        {
            var plan = await _context.Plans.FindAsync(planId);
            if (plan == null) return false;

            _context.Plans.Remove(plan);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddPlanItemAsync(PlanList planItem)
        {
            await _context.PlanLists.AddAsync(planItem);
            
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await UpdatePlanTotalCost(planItem.PlanId);
                return true;
            }
            return false;
        }

        public async Task<bool> UpdatePlanItemAsync(PlanList planItem)
        {
            var existingItem = await _context.PlanLists.FindAsync(planItem.PlanListId);
            if (existingItem == null) return false;

            existingItem.DayNumber = planItem.DayNumber;
            existingItem.ItemNo = planItem.ItemNo;
            existingItem.Price = planItem.Price;

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await UpdatePlanTotalCost(planItem.PlanId);
                return true;
            }
            return false;
        }

        public async Task<bool> DeletePlanItemAsync(int planId, int itemId)
        {
            var planItem = await _context.PlanLists
                .FirstOrDefaultAsync(pl => pl.PlanId == planId && pl.ItemId == itemId);
            if (planItem == null) return false;

            _context.PlanLists.Remove(planItem);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await UpdatePlanTotalCost(planId);
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<PlanList>> GetPlanItemsAsync(int planId)
        {
            return await _context.PlanLists
                .Include(pl => pl.Item)
                .Where(pl => pl.PlanId == planId)
                .OrderBy(pl => pl.DayNumber)
                .ThenBy(pl => pl.ItemNo)
                .ToListAsync();
        }

        public async Task<decimal> CalculateTotalCostAsync(int planId)
        {
            return await _context.PlanLists
                .Where(pl => pl.PlanId == planId)
                .SumAsync(pl => pl.Price ?? 0m);
        }

        private async Task UpdatePlanTotalCost(int planId)
        {
            var totalCost = await CalculateTotalCostAsync(planId);
            var plan = await _context.Plans.FindAsync(planId);
            if (plan != null)
            {
                plan.TotalCost = totalCost;
                plan.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
} 