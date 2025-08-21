using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using PlanyApp.Repository.Interfaces;
using PlanyApp.Repository.Models;
using PlanyApp.Repository.UnitOfWork;
using PlanyApp.Service.Dto.Plan;
using PlanyApp.Service.Interfaces;
using System.Security.Claims;

namespace PlanyApp.Service.Services
{
    public class PlanService : IPlanService
    {
        private readonly IPlanRepository _planRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IProvinceDetectionService _provinceDetectionService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PlanService(IPlanRepository planRepository, IItemRepository itemRepository, IProvinceDetectionService provinceDetectionService, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _planRepository = planRepository;
            _itemRepository = itemRepository;
            _provinceDetectionService = provinceDetectionService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
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
            // Detect province from plan name
            var detectedProvinceId = await _provinceDetectionService.DetectProvinceFromNameAsync(createPlanDto.Name);

            // Manual mapping to avoid AutoMapper issues
            var plan = new Plan
            {
                Name = createPlanDto.Name,
                StartDate = createPlanDto.StartDate.HasValue ? DateOnly.FromDateTime(createPlanDto.StartDate.Value) : null,
                EndDate = createPlanDto.EndDate.HasValue ? DateOnly.FromDateTime(createPlanDto.EndDate.Value) : null,
                IsPublic = createPlanDto.IsPublic,
                ProvinceId = detectedProvinceId,
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PlanLists = new List<PlanList>()
            };

            if (plan.StartDate.HasValue && plan.EndDate.HasValue)
            {
                if (plan.EndDate.Value < plan.StartDate.Value)
                {
                    throw new ArgumentException("End date cannot be earlier than start date.");
                }

                var daysDifference = plan.EndDate.Value.DayNumber - plan.StartDate.Value.DayNumber;
                plan.NightCount = daysDifference;
                plan.DayCount = daysDifference + 1;
            }
            else
            {
                plan.DayCount = 1;
                plan.NightCount = 0;
            }

            decimal totalCost = 0;
            
            // ALWAYS process items if they exist, regardless of totalCost
            if (createPlanDto.Items != null && createPlanDto.Items.Count > 0)
            {
                var allowedItemTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Hotel", "Transportation", "Place" };

                foreach (var itemDto in createPlanDto.Items)
                {
                    if (itemDto.ItemType != null && !allowedItemTypes.Contains(itemDto.ItemType))
                    {
                        throw new ArgumentException($"Invalid item type: {itemDto.ItemType}. Allowed types are Hotel, Transportation, Place.");
                    }

                    int itemId;

                    if (!itemDto.ItemId.HasValue)
                    {
                        // This is a custom item, create a new Item entity for it
                        if (string.IsNullOrWhiteSpace(itemDto.Name) || string.IsNullOrWhiteSpace(itemDto.ItemType))
                        {
                            throw new ArgumentException("New items must have a name and a type.");
                        }
                        var newItem = new Item
                        {
                            Name = itemDto.Name,
                            ItemType = itemDto.ItemType,
                            Description = itemDto.Description,
                            Address = "TBD", // Default address for custom items
                            Price = itemDto.Price,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            IsActive = true
                        };
                        var createdItem = await _itemRepository.CreateAsync(newItem);
                        itemId = createdItem.ItemId;
                    }
                    else
                    {
                        // Validate that the existing item exists
                        var existingItem = await _itemRepository.GetByIdAsync(itemDto.ItemId.Value);
                        if (existingItem == null)
                        {
                            throw new ArgumentException($"Item with ID {itemDto.ItemId.Value} does not exist.");
                        }
                        itemId = itemDto.ItemId.Value;
                    }

                    // Create PlanList entity with validation
                    var planList = new PlanList
                    {
                        // PlanListId will be auto-generated by database
                        ItemId = itemId,
                        DayNumber = itemDto.DayNumber > 0 ? itemDto.DayNumber : 1, // Default to day 1 if invalid
                        ItemNo = itemDto.ItemNo > 0 ? itemDto.ItemNo : 1, // Default to position 1 if invalid
                        StartTime = itemDto.StartTime,
                        EndTime = itemDto.EndTime,
                        Notes = itemDto.Notes ?? string.Empty,
                        Price = itemDto.Price ?? 0
                    };
                    
                    plan.PlanLists.Add(planList);
                    totalCost += itemDto.Price ?? 0;
                }
            }
            
            // Use provided totalCost or calculated from items
            plan.TotalCost = createPlanDto.TotalCost ?? totalCost;

            var createdPlan = await _planRepository.CreatePlanAsync(plan);

            // Audit (with error handling)
            try
            {
                await _unitOfWork.PlanAuditLogRepository.AddAsync(new PlanAuditLog
                {
                    PlanId = createdPlan.PlanId,
                    UserId = ownerId,
                    Action = "CreatePlan",
                    ChangesJson = null,
                    CreatedAt = DateTime.UtcNow
                });
                await _unitOfWork.SaveAsync();
            }
            catch (Exception auditEx)
            {
                // Log the error but don't fail the plan creation
                Console.WriteLine($"Failed to save audit log: {auditEx.Message}");
                // Continue without failing - audit is not critical
            }

            return _mapper.Map<PlanDto>(createdPlan);
        }

        public async Task<PlanDto> UpdatePlanAsync(int planId, UpdatePlanDto updatePlanDto)
        {
            var existingPlan = await _planRepository.GetPlanByIdAsync(planId);
            if (existingPlan == null) return null;

            var updatedPlan = _mapper.Map(updatePlanDto, existingPlan);
            
            // If the name was updated, try to detect province again
            if (!string.IsNullOrWhiteSpace(updatePlanDto.Name))
            {
                var detectedProvinceId = await _provinceDetectionService.DetectProvinceFromNameAsync(updatePlanDto.Name);
                updatedPlan.ProvinceId = detectedProvinceId;
            }

            var result = await _planRepository.UpdatePlanAsync(updatedPlan);

            // Current user id from HTTP context
            var userIdStr = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            int.TryParse(userIdStr, out var userId);

            await _unitOfWork.PlanAuditLogRepository.AddAsync(new PlanAuditLog
            {
                PlanId = planId,
                UserId = userId,
                Action = "UpdatePlan",
                ChangesJson = null,
                CreatedAt = DateTime.UtcNow
            });
            await _unitOfWork.SaveAsync();

            return _mapper.Map<PlanDto>(result);
        }

        public async Task<bool> DeletePlanAsync(int planId)
        {
            var success = await _planRepository.DeletePlanAsync(planId);
            if (success)
            {
                var userIdStr = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);

                await _unitOfWork.PlanAuditLogRepository.AddAsync(new PlanAuditLog
                {
                    PlanId = planId,
                    UserId = userId,
                    Action = "DeletePlan",
                    ChangesJson = null,
                    CreatedAt = DateTime.UtcNow
                });
                await _unitOfWork.SaveAsync();
            }
            return success;
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

            var userIdStr = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            int.TryParse(userIdStr, out var userId);

            await _unitOfWork.PlanAuditLogRepository.AddAsync(new PlanAuditLog
            {
                PlanId = planId,
                UserId = userId,
                Action = "UpdatePlanItem",
                ChangesJson = null,
                CreatedAt = DateTime.UtcNow
            });
            await _unitOfWork.SaveAsync();

            return await GetPlanByIdAsync(planId);
        }

        public async Task<bool> DeletePlanItemAsync(int planId, int itemId)
        {
            var success = await _planRepository.DeletePlanItemAsync(planId, itemId);
            if (success)
            {
                var userIdStr = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdStr, out var userId);

                await _unitOfWork.PlanAuditLogRepository.AddAsync(new PlanAuditLog
                {
                    PlanId = planId,
                    UserId = userId,
                    Action = "DeletePlanItem",
                    ChangesJson = null,
                    CreatedAt = DateTime.UtcNow
                });
                await _unitOfWork.SaveAsync();
            }
            return success;
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
