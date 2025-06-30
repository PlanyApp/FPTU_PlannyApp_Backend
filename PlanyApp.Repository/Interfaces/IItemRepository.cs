using PlanyApp.Repository.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanyApp.Repository.Interfaces
{
    public interface IItemRepository
    {
        Task<IEnumerable<Item>> GetAllAsync();
        Task<Item?> GetByIdAsync(int id);
        Task<IEnumerable<Item>> GetByTypeAsync(string itemType);
        Task<Hotel?> GetHotelByIdAsync(int itemId);
        Task<Place?> GetPlaceByIdAsync(int itemId);
        Task<Transportation?> GetTransportationByIdAsync(int itemId);
        Task<IEnumerable<Hotel>> GetAllHotelsAsync();
        Task<IEnumerable<Place>> GetAllPlacesAsync();
        Task<IEnumerable<Transportation>> GetAllTransportationsAsync();
        Task<IEnumerable<Hotel>> SearchHotelsByNameAsync(string name);
        Task<IEnumerable<Place>> SearchPlacesByNameAsync(string name);
        Task<IEnumerable<Transportation>> SearchTransportationsByNameAsync(string name);
        Task<Item> CreateAsync(Item item);
    }
} 