// IMaintenanceRepository.cs
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IMaintenanceRepository : IGenericRepository<Maintenance>
    {
        Task<IEnumerable<Maintenance>> GetByInventoryItemIdAsync(int inventoryItemId);
    }
}