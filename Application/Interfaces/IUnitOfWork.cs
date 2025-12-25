using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IEmployeeRepository Employees { get; }
        IInternRepository Interns { get; }
        IDepartmentRepository Departments { get; }
        IUniversityRepository Universities { get; }
        IInventoryItemRepository InventoryItems { get; }
        IAssignmentRepository Assignments { get; }
        IMaintenanceRepository Maintenances { get; }

        IGenericRepository<AppUser> AppUsers { get; }

        Task<int> CommitAsync();
    }
}