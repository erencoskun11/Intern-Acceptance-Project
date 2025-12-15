// IInventoryItemRepository.cs
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IInventoryItemRepository : IGenericRepository<InventoryItem>
    {
        IQueryable<InventoryItem> GetAvailableItems();
    }
}