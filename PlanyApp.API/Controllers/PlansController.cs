using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanyApp.Service.Dto.Plan;
using PlanyApp.Service.Interfaces;

namespace PlanyApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PlansController : ControllerBase
    {
        private readonly IPlanService _planService;

        public PlansController(IPlanService planService)
        {
            _planService = planService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlanDto>>> GetAllPlans()
        {
            var plans = await _planService.GetAllPlansAsync();
            return Ok(plans);
        }

        [HttpGet("{planId}")]
        public async Task<ActionResult<PlanDto>> GetPlanById(int planId)
        {
            var plan = await _planService.GetPlanByIdAsync(planId);
            if (plan == null) return NotFound();
            return Ok(plan);
        }

        [HttpGet("user/{ownerId}")]
        public async Task<ActionResult<IEnumerable<PlanDto>>> GetPlansByOwnerId(int ownerId)
        {
            var plans = await _planService.GetPlansByOwnerIdAsync(ownerId);
            return Ok(plans);
        }

        [HttpPost]
        public async Task<ActionResult<PlanDto>> CreatePlan(CreatePlanDto createPlanDto)
        {
            var plan = await _planService.CreatePlanAsync(createPlanDto);
            return CreatedAtAction(nameof(GetPlanById), new { planId = plan.PlanId }, plan);
        }

        [HttpPut("{planId}")]
        public async Task<ActionResult<PlanDto>> UpdatePlan(int planId, UpdatePlanDto updatePlanDto)
        {
            var plan = await _planService.UpdatePlanAsync(planId, updatePlanDto);
            if (plan == null) return NotFound();
            return Ok(plan);
        }

        [HttpDelete("{planId}")]
        public async Task<ActionResult> DeletePlan(int planId)
        {
            var result = await _planService.DeletePlanAsync(planId);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("{planId}/items")]
        public async Task<ActionResult<IEnumerable<PlanListDto>>> GetPlanItems(int planId)
        {
            var items = await _planService.GetPlanItemsAsync(planId);
            return Ok(items);
        }

        [HttpPost("{planId}/items")]
        public async Task<ActionResult<PlanDto>> AddPlanItem(int planId, CreatePlanListDto createPlanListDto)
        {
            var plan = await _planService.AddPlanItemAsync(planId, createPlanListDto);
            if (plan == null) return NotFound();
            return Ok(plan);
        }

        [HttpPut("{planId}/items/{planListId}")]
        public async Task<ActionResult<PlanDto>> UpdatePlanItem(int planId, int planListId, UpdatePlanListDto updatePlanListDto)
        {
            var plan = await _planService.UpdatePlanItemAsync(planId, planListId, updatePlanListDto);
            if (plan == null) return NotFound();
            return Ok(plan);
        }

        [HttpDelete("{planId}/items/{itemId}")]
        public async Task<ActionResult> DeletePlanItem(int planId, int itemId)
        {
            var result = await _planService.DeletePlanItemAsync(planId, itemId);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("{planId}/total-cost")]
        public async Task<ActionResult<decimal>> GetPlanTotalCost(int planId)
        {
            var totalCost = await _planService.GetPlanTotalCostAsync(planId);
            return Ok(totalCost);
        }
    }
} 