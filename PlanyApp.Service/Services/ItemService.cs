using PlanyApp.Repository.Interfaces;
using PlanyApp.Service.Dto.Items;
using PlanyApp.Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace PlanyApp.Service.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;

        public ItemService(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<IEnumerable<ItemDto>> GetAllItemsAsync()
        {
            var items = await _itemRepository.GetAllAsync();
            return items.Select(i => new ItemDto
            {
                ItemId = i.ItemId,
                ItemType = i.ItemType,
                CreatedAt = i.CreatedAt
            });
        }

        public async Task<IEnumerable<ItemDto>> GetItemsByTypeAsync(string itemType)
        {
            var items = await _itemRepository.GetByTypeAsync(itemType);
            return items.Select(i => new ItemDto
            {
                ItemId = i.ItemId,
                ItemType = i.ItemType,
                CreatedAt = i.CreatedAt
            });
        }

        public async Task<ItemDto?> GetItemByIdAsync(int id)
        {
            var item = await _itemRepository.GetByIdAsync(id);
            if (item == null) return null;

            return new ItemDto
            {
                ItemId = item.ItemId,
                ItemType = item.ItemType,
                CreatedAt = item.CreatedAt
            };
        }

        public async Task<IEnumerable<HotelDto>> GetAllHotelsAsync()
        {
            var hotels = await _itemRepository.GetAllHotelsAsync();
            return hotels.Select(h => new HotelDto
            {
                ItemId = h.ItemId,
                ItemType = h.Item.ItemType,
                CreatedAt = h.Item.CreatedAt,
                Name = h.Name,
                Address = h.Address,
                Latitude = h.Latitude,
                Longitude = h.Longitude,
                CheckInTime = h.CheckInTime,
                CheckOutTime = h.CheckOutTime
            });
        }

        public async Task<HotelDto?> GetHotelByIdAsync(int itemId)
        {
            var hotel = await _itemRepository.GetHotelByIdAsync(itemId);
            if (hotel == null) return null;

            return new HotelDto
            {
                ItemId = hotel.ItemId,
                ItemType = hotel.Item.ItemType,
                CreatedAt = hotel.Item.CreatedAt,
                Name = hotel.Name,
                Address = hotel.Address,
                Latitude = hotel.Latitude,
                Longitude = hotel.Longitude,
                CheckInTime = hotel.CheckInTime,
                CheckOutTime = hotel.CheckOutTime
            };
        }

        public async Task<IEnumerable<PlaceDto>> GetAllPlacesAsync()
        {
            var places = await _itemRepository.GetAllPlacesAsync();
            return places.Select(p => new PlaceDto
            {
                ItemId = p.ItemId,
                ItemType = p.Item.ItemType,
                CreatedAt = p.Item.CreatedAt,
                Name = p.Name,
                Address = p.Address,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                Price = p.Price,
                Description = p.Description,
                OpenTime = p.OpenTime,
                CloseTime = p.CloseTime
            });
        }

        public async Task<PlaceDto?> GetPlaceByIdAsync(int itemId)
        {
            var place = await _itemRepository.GetPlaceByIdAsync(itemId);
            if (place == null) return null;

            return new PlaceDto
            {
                ItemId = place.ItemId,
                ItemType = place.Item.ItemType,
                CreatedAt = place.Item.CreatedAt,
                Name = place.Name,
                Address = place.Address,
                Latitude = place.Latitude,
                Longitude = place.Longitude,
                Price = place.Price,
                Description = place.Description,
                OpenTime = place.OpenTime,
                CloseTime = place.CloseTime
            };
        }

        public async Task<IEnumerable<TransportationDto>> GetAllTransportationsAsync()
        {
            var transportations = await _itemRepository.GetAllTransportationsAsync();
            return transportations.Select(t => new TransportationDto
            {
                ItemId = t.ItemId,
                ItemType = t.Item.ItemType,
                CreatedAt = t.Item.CreatedAt,
                Name = t.Name,
                Address = t.Address,
                Latitude = t.Latitude,
                Longitude = t.Longitude,
                OpenTime = t.OpenTime,
                CloseTime = t.CloseTime
            });
        }

        public async Task<TransportationDto?> GetTransportationByIdAsync(int itemId)
        {
            var transportation = await _itemRepository.GetTransportationByIdAsync(itemId);
            if (transportation == null) return null;

            return new TransportationDto
            {
                ItemId = transportation.ItemId,
                ItemType = transportation.Item.ItemType,
                CreatedAt = transportation.Item.CreatedAt,
                Name = transportation.Name,
                Address = transportation.Address,
                Latitude = transportation.Latitude,
                Longitude = transportation.Longitude,
                OpenTime = transportation.OpenTime,
                CloseTime = transportation.CloseTime
            };
        }

        public async Task<IEnumerable<HotelDto>> SearchHotelsByNameAsync(string name)
        {
            var hotels = await _itemRepository.SearchHotelsByNameAsync(name);
            return hotels.Select(h => new HotelDto
            {
                ItemId = h.ItemId,
                ItemType = h.Item.ItemType,
                CreatedAt = h.Item.CreatedAt,
                Name = h.Name,
                Address = h.Address,
                Latitude = h.Latitude,
                Longitude = h.Longitude,
                CheckInTime = h.CheckInTime,
                CheckOutTime = h.CheckOutTime
            });
        }

        public async Task<IEnumerable<PlaceDto>> SearchPlacesByNameAsync(string name)
        {
            var places = await _itemRepository.SearchPlacesByNameAsync(name);
            return places.Select(p => new PlaceDto
            {
                ItemId = p.ItemId,
                ItemType = p.Item.ItemType,
                CreatedAt = p.Item.CreatedAt,
                Name = p.Name,
                Address = p.Address,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                Price = p.Price,
                Description = p.Description,
                OpenTime = p.OpenTime,
                CloseTime = p.CloseTime
            });
        }

        public async Task<IEnumerable<TransportationDto>> SearchTransportationsByNameAsync(string name)
        {
            var transportations = await _itemRepository.SearchTransportationsByNameAsync(name);
            return transportations.Select(t => new TransportationDto
            {
                ItemId = t.ItemId,
                ItemType = t.Item.ItemType,
                CreatedAt = t.Item.CreatedAt,
                Name = t.Name,
                Address = t.Address,
                Latitude = t.Latitude,
                Longitude = t.Longitude,
                OpenTime = t.OpenTime,
                CloseTime = t.CloseTime
            });
        }
    }
} 