using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanyApp.Service.Dto.Items;
using PlanyApp.Service.Interfaces;
using System.Threading.Tasks;

namespace PlanyApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _itemService.GetAllItemsAsync();
            return Ok(items);
        }

        [HttpGet("type/{itemType}")]
        public async Task<IActionResult> GetItemsByType(string itemType)
        {
            var items = await _itemService.GetItemsByTypeAsync(itemType);
            return Ok(items);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetItemsByCategory(int categoryId)
        {
            var items = await _itemService.GetItemsByCategoryAsync(categoryId);
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemById(int id)
        {
            var item = await _itemService.GetItemByIdAsync(id);
            if (item == null)
                return NotFound();
            return Ok(item);
        }

        [HttpGet("hotels")]
        public async Task<IActionResult> GetAllHotels()
        {
            var hotels = await _itemService.GetAllHotelsAsync();
            return Ok(hotels);
        }

        [HttpGet("hotels/{id}")]
        public async Task<IActionResult> GetHotelById(int id)
        {
            var hotel = await _itemService.GetHotelByIdAsync(id);
            if (hotel == null)
                return NotFound();
            return Ok(hotel);
        }

        [HttpGet("places")]
        public async Task<IActionResult> GetAllPlaces()
        {
            var places = await _itemService.GetAllPlacesAsync();
            return Ok(places);
        }

        [HttpGet("places/{id}")]
        public async Task<IActionResult> GetPlaceById(int id)
        {
            var place = await _itemService.GetPlaceByIdAsync(id);
            if (place == null)
                return NotFound();
            return Ok(place);
        }

        [HttpGet("transportations")]
        public async Task<IActionResult> GetAllTransportations()
        {
            var transportations = await _itemService.GetAllTransportationsAsync();
            return Ok(transportations);
        }

        [HttpGet("transportations/{id}")]
        public async Task<IActionResult> GetTransportationById(int id)
        {
            var transportation = await _itemService.GetTransportationByIdAsync(id);
            if (transportation == null)
                return NotFound();
            return Ok(transportation);
        }
    }
} 