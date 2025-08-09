using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanyApp.Service.Dto.Plan;
using PlanyApp.Service.Interfaces;
using System;
using System.Linq;
using PlanyApp.API.Models;
using System.Security.Claims;

namespace PlanyApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PlansController : ControllerBase
    {
        private readonly IPlanService _planService;
        private readonly IPlanAccessService _planAccessService;

        public PlansController(IPlanService planService, IPlanAccessService planAccessService)
        {
            _planService = planService;
            _planAccessService = planAccessService;
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
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var canView = await _planAccessService.CanViewPlanAsync(userId, planId);
            if (!canView) return Forbid();

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
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var canEdit = await _planAccessService.CanEditPlanAsync(userId, planId);
            if (!canEdit) return Forbid();

            var plan = await _planService.UpdatePlanAsync(planId, updatePlanDto);
            if (plan == null) return NotFound();
            return Ok(plan);
        }

        [HttpDelete("{planId}")]
        public async Task<ActionResult> DeletePlan(int planId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var canEdit = await _planAccessService.CanEditPlanAsync(userId, planId);
            if (!canEdit) return Forbid();

            var result = await _planService.DeletePlanAsync(planId);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("{planId}/items")]
        public async Task<ActionResult<IEnumerable<PlanListDto>>> GetPlanItems(int planId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var canView = await _planAccessService.CanViewPlanAsync(userId, planId);
            if (!canView) return Forbid();

            var items = await _planService.GetPlanItemsAsync(planId);
            return Ok(items);
        }

        [HttpPut("{planId}/items/{planListId}")]
        public async Task<ActionResult<PlanDto>> UpdatePlanItem(int planId, int planListId, UpdatePlanListDto updatePlanListDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var canEdit = await _planAccessService.CanEditPlanAsync(userId, planId);
            if (!canEdit) return Forbid();

            var plan = await _planService.UpdatePlanItemAsync(planId, planListId, updatePlanListDto);
            if (plan == null) return NotFound();
            return Ok(plan);
        }

        [HttpDelete("{planId}/items/{itemId}")]
        public async Task<ActionResult> DeletePlanItem(int planId, int itemId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var canEdit = await _planAccessService.CanEditPlanAsync(userId, planId);
            if (!canEdit) return Forbid();

            var result = await _planService.DeletePlanItemAsync(planId, itemId);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("{planId}/total-cost")]
        public async Task<ActionResult<decimal>> GetPlanTotalCost(int planId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var canView = await _planAccessService.CanViewPlanAsync(userId, planId);
            if (!canView) return Forbid();

            var totalCost = await _planService.GetPlanTotalCostAsync(planId);
            return Ok(totalCost);
        }

        // New: Link a plan to a group so group members can access/edit
        [HttpPost("{planId}/link-group/{groupId}")]
        public async Task<IActionResult> LinkPlanToGroup(int planId, int groupId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var success = await _planAccessService.LinkPlanToGroupAsync(planId, groupId, userId);
            if (!success) return Forbid();
            return Ok(ApiResponse<object>.SuccessResponse(null, "Plan linked to group successfully."));
        }
    }
} 