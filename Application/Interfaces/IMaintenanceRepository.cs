using Domain.Entities;
using System.Linq;

namespace Application.Interfaces
{
    public interface IMaintenanceRepository : IGenericRepository<Maintenance>
    {
        // IQueryable dönecek (Veritabanına gitmez, sorgu hazırlar)
        IQueryable<Maintenance> GetByInventoryItemId(int itemId);
        IQueryable<Maintenance> GetAllWithDetails();
    }
}