// InventoryItemRepository.cs
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;

namespace Infrastructure.Repositories
{
    public class InventoryItemRepository : GenericRepository<InventoryItem>, IInventoryItemRepository
    {
        public InventoryItemRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<InventoryItem>> GetAvailableItemsAsync() =>
            await _context.InventoryItems.Where(i => i.Status == Domain.Enums.ItemStatus.Available).ToListAsync();
    }
}
