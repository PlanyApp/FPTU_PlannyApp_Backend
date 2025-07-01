using System.Collections.Generic;
using System.Security.Claims;
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

        /// <summary>
        /// Creates a new travel plan.
        /// </summary>
        /// <param name="createPlanDto">The plan creation request data.</param>
        /// <returns>The newly created plan.</returns>
        /// <remarks>
        /// This endpoint allows you to create a new travel plan with a start date, end date, and a list of items.
        /// 
        /// **Date Handling:**
        /// - `startDate` and `endDate` are used to calculate `DayCount` and `NightCount`.
        /// - If no dates are provided, the plan is considered a single-day plan.
        /// 
        /// **Item Handling:**
        /// - To add an existing item, provide its `itemId`.
        /// - To create a new custom item, set `itemId` to `null` and provide a `name` and `itemType`.
        /// - `itemType` must be one of "Hotel", "Transportation", or "Place" (case-insensitive).
        /// 
        /// **Sample request:**
        ///
        ///     POST /api/Plans
        ///     {
        ///       "name": "My Awesome Trip",
        ///       "startDate": "2025-08-25T00:00:00Z",
        ///       "endDate": "2025-08-28T00:00:00Z",
        ///       "isPublic": true,
        ///       "items": [
        ///         {
        ///           "itemId": 1,
        ///           "name": null,
        ///           "itemType": "Hotel",
        ///           "dayNumber": 1,
        ///           "itemNo": 1,
        ///           "startTime": "14:00:00",
        ///           "endTime": "15:00:00",
        ///           "notes": "Check-in",
        ///           "price": 150.00
        ///         },
        ///         {
        ///           "itemId": null,
        ///           "name": "Local Museum Visit",
        ///           "itemType": "Place",
        ///           "dayNumber": 2,
        ///           "itemNo": 1,
        ///           "startTime": "10:00:00",
        ///           "endTime": "12:00:00",
        ///           "notes": "Explore local history",
        ///           "price": 25.00
        ///         }
        ///       ]
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Returns the newly created plan.</response>
        /// <response code="400">If the request is invalid (e.g., missing required fields, invalid item type).</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PlanDto>> CreatePlan(CreatePlanRequestDto createPlanDto)
        {
            var ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var plan = await _planService.CreatePlanAsync(createPlanDto, ownerId);
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