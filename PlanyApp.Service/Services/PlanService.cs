using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PlanyApp.Repository.Interfaces;
using PlanyApp.Repository.Models;
using PlanyApp.Service.Dto.Plan;
using PlanyApp.Service.Interfaces;

namespace PlanyApp.Service.Services
{
    public class PlanService : IPlanService
    {
        private readonly IPlanRepository _planRepository;
        private readonly IMapper _mapper;

        public PlanService(IPlanRepository planRepository, IMapper mapper)
        {
            _planRepository = planRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PlanDto>> GetAllPlansAsync()
        {
            var plans = await _planRepository.GetAllPlansAsync();
            return _mapper.Map<IEnumerable<PlanDto>>(plans);
        }

        public async Task<PlanDto> GetPlanByIdAsync(int planId)
        {
            var plan = await _planRepository.GetPlanByIdAsync(planId);
            return _mapper.Map<PlanDto>(plan);
        }

        public async Task<IEnumerable<PlanDto>> GetPlansByOwnerIdAsync(int ownerId)
        {
            var plans = await _planRepository.GetPlansByOwnerIdAsync(ownerId);
            return _mapper.Map<IEnumerable<PlanDto>>(plans);
        }

        public async Task<PlanDto> CreatePlanAsync(CreatePlanRequestDto createPlanDto, int ownerId)
        {
            var plan = _mapper.Map<Plan>(createPlanDto);
            plan.OwnerId = ownerId;
            plan.CreatedAt = DateTime.UtcNow;
            plan.UpdatedAt = DateTime.UtcNow;

            if (plan.StartDate.HasValue && plan.EndDate.HasValue)
            {
                plan.DayCount = (plan.EndDate.Value.DayNumber - plan.StartDate.Value.DayNumber) + 1;
                plan.NightCount = plan.DayCount > 0 ? plan.DayCount - 1 : 0;
            }

            decimal totalCost = 0;
            foreach (var itemDto in createPlanDto.Items)
            {
                var planList = _mapper.Map<PlanList>(itemDto);
                if (!planList.ItemId.HasValue)
                {
                    // This is a custom item, we may need to create a new Item entity for it
                    // For now, we'll just save the notes and price.
                }
                plan.PlanLists.Add(planList);
                totalCost += itemDto.Price ?? 0;
            }
            plan.TotalCost = totalCost;

            var createdPlan = await _planRepository.CreatePlanAsync(plan);
            return _mapper.Map<PlanDto>(createdPlan);
        }

        public async Task<PlanDto> UpdatePlanAsync(int planId, UpdatePlanDto updatePlanDto)
        {
            var existingPlan = await _planRepository.GetPlanByIdAsync(planId);
            if (existingPlan == null) return null;

            var updatedPlan = _mapper.Map(updatePlanDto, existingPlan);
            var result = await _planRepository.UpdatePlanAsync(updatedPlan);
            return _mapper.Map<PlanDto>(result);
        }

        public async Task<bool> DeletePlanAsync(int planId)
        {
            return await _planRepository.DeletePlanAsync(planId);
        }

        public async Task<PlanDto> UpdatePlanItemAsync(int planId, int planListId, UpdatePlanListDto updatePlanListDto)
        {
            var planItem = new PlanList
            {
                PlanListId = planListId,
                PlanId = planId,
                DayNumber = updatePlanListDto.DayNumber,
                ItemNo = updatePlanListDto.ItemNo,
                Price = updatePlanListDto.Price
            };

            var success = await _planRepository.UpdatePlanItemAsync(planItem);
            if (!success) return null;

            return await GetPlanByIdAsync(planId);
        }

        public async Task<bool> DeletePlanItemAsync(int planId, int itemId)
        {
            return await _planRepository.DeletePlanItemAsync(planId, itemId);
        }

        public async Task<IEnumerable<PlanListDto>> GetPlanItemsAsync(int planId)
        {
            var planItems = await _planRepository.GetPlanItemsAsync(planId);
            return _mapper.Map<IEnumerable<PlanListDto>>(planItems);
        }

        public async Task<decimal> GetPlanTotalCostAsync(int planId)
        {
            return await _planRepository.CalculateTotalCostAsync(planId);
        }
    }
} 