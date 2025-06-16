using Microsoft.EntityFrameworkCore;
using PlanyApp.Repository.Interfaces;
using PlanyApp.Repository.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanyApp.Repository.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly PlanyDBContext _context;

        public ItemRepository(PlanyDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Item>> GetAllAsync()
        {
            return await _context.Items.ToListAsync();
        }

        public async Task<IEnumerable<Item>> GetByTypeAsync(string itemType)
        {
            return await _context.Items
                .Where(i => i.ItemType == itemType)
                .ToListAsync();
        }

        public async Task<Item?> GetByIdAsync(int id)
        {
            return await _context.Items.FindAsync(id);
        }

        public async Task<Hotel?> GetHotelByIdAsync(int itemId)
        {
            return await _context.Hotels
                .Include(h => h.Item)
                .FirstOrDefaultAsync(h => h.ItemId == itemId);
        }

        public async Task<Place?> GetPlaceByIdAsync(int itemId)
        {
            return await _context.Places
                .Include(p => p.Item)
                .FirstOrDefaultAsync(p => p.ItemId == itemId);
        }

        public async Task<Transportation?> GetTransportationByIdAsync(int itemId)
        {
            return await _context.Transportations
                .Include(t => t.Item)
                .FirstOrDefaultAsync(t => t.ItemId == itemId);
        }

        public async Task<IEnumerable<Hotel>> GetAllHotelsAsync()
        {
            return await _context.Hotels
                .Include(h => h.Item)
                .ToListAsync();
        }

        public async Task<IEnumerable<Place>> GetAllPlacesAsync()
        {
            return await _context.Places
                .Include(p => p.Item)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transportation>> GetAllTransportationsAsync()
        {
            return await _context.Transportations
                .Include(t => t.Item)
                .ToListAsync();
        }

        public async Task<IEnumerable<Hotel>> SearchHotelsByNameAsync(string name)
        {
            return await _context.Hotels
                .Include(h => h.Item)
                .Where(h => h.Name.Contains(name))
                .ToListAsync();
        }

        public async Task<IEnumerable<Place>> SearchPlacesByNameAsync(string name)
        {
            return await _context.Places
                .Include(p => p.Item)
                .Where(p => p.Name.Contains(name))
                .ToListAsync();
        }

        public async Task<IEnumerable<Transportation>> SearchTransportationsByNameAsync(string name)
        {
            return await _context.Transportations
                .Include(t => t.Item)
                .Where(t => t.Name.Contains(name))
                .ToListAsync();
        }
    }
} 