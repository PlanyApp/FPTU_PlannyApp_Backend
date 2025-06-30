using Microsoft.AspNetCore.Mvc;
using PlanyApp.API.Models;
using PlanyApp.Repository.Models;
using PlanyApp.Service.Dto.Gift;
using PlanyApp.Service.Interfaces;

namespace PlanyApp.API.Controllers
{
    [ApiController]
    [Route("api/gifts")]
    public class GiftsController : ControllerBase
    {
        private readonly IGiftService _giftService;

        public GiftsController(IGiftService giftService)
        {
            _giftService = giftService;
        }

        // GET: /api/gifts
        [HttpGet]
        public async Task<IActionResult> GetAllGifts()
        {
            var gifts = await _giftService.GetAllGiftsAsync();
            return Ok(ApiResponse<List<Gift>>.SuccessResponse(gifts));
        }

        // GET: /api/gifts/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGiftById(int id)
        {
            var gift = await _giftService.GetGiftByIdAsync(id);
            if (gift == null)
                return NotFound(ApiResponse<string>.ErrorResponse("Gift not found"));

            return Ok(ApiResponse<Gift>.SuccessResponse(gift));
        }

        // GET: /api/gifts/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetGiftsByUserId(int userId)
        {
            var userGifts = await _giftService.GetGiftsByUserIdAsync(userId);
            return Ok(ApiResponse<List<Gift>>.SuccessResponse(userGifts));
        }
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
