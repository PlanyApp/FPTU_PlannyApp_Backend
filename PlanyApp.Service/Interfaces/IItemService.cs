using PlanyApp.Service.Dto;
using PlanyApp.Service.Dto.Items;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanyApp.Service.Interfaces
{
    public interface IItemService
    {
        Task<IEnumerable<ItemDto>> GetAllItemsAsync();
        Task<IEnumerable<ItemDto>> GetItemsByTypeAsync(string itemType);
        Task<IEnumerable<ItemDto>> GetItemsByCategoryAsync(int categoryId);
        Task<ItemDto?> GetItemByIdAsync(int id);
        
        Task<IEnumerable<HotelDto>> GetAllHotelsAsync();
        Task<HotelDto?> GetHotelByIdAsync(int itemId);
        
        Task<IEnumerable<PlaceDto>> GetAllPlacesAsync();
        Task<PlaceDto?> GetPlaceByIdAsync(int itemId);
        
        Task<IEnumerable<TransportationDto>> GetAllTransportationsAsync();
        Task<TransportationDto?> GetTransportationByIdAsync(int itemId);
    }
} 