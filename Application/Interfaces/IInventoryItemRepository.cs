// IInventoryItemRepository.cs
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IInventoryItemRepository : IGenericRepository<InventoryItem>
    {
        Task<IEnumerable<InventoryItem>> GetAvailableItemsAsync();
    }
}