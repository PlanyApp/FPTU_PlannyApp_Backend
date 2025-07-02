using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanyApp.API.Models;
using PlanyApp.Repository.Models;
using PlanyApp.Service.Dto.Gift;
using PlanyApp.Service.Interfaces;

namespace PlanyApp.API.Controllers
{
    [ApiController]
    [Route("api/gifts")]
    [Authorize]
    public class GiftsController : ControllerBase
    {
        private readonly IGiftService _giftService;

        public GiftsController(IGiftService giftService)
        {
            _giftService = giftService;
        }

        /// <summary>
        /// Get all list gifts
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllGifts()
        {
            var gifts = await _giftService.GetAllGiftsAsync();
            return Ok(ApiResponse<List<Gift>>.SuccessResponse(gifts));
        }

        /// <summary>
        /// Get gift by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGiftById(int id)
        {
            var gift = await _giftService.GetGiftByIdAsync(id);
            if (gift == null)
                return NotFound(ApiResponse<string>.ErrorResponse("Gift not found"));

            return Ok(ApiResponse<Gift>.SuccessResponse(gift));
        }

        /// <summary>
        /// Get gifts of a user by user ID
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetGiftsByUserId(int userId)
        {
            var userGifts = await _giftService.GetGiftsByUserIdAsync(userId);
            return Ok(ApiResponse<List<Gift>>.SuccessResponse(userGifts));
        }
        /// <summary>
        /// Redeem a gift by user ID and gift ID
        /// </summary>
        [HttpPost("redeem")]
        public async Task<IActionResult> RedeemGift([FromQuery] int userId, [FromQuery] int giftId)
        {
            try
            {
                var response = await _giftService.RedeemGiftAsync(userId, giftId);
                return Ok(ApiResponse<RedeemGiftResponse>.SuccessResponse(response));
            }
            catch (Exception ex)
            {
                return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

    }
}
