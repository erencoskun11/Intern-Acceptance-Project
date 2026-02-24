using Application.Interfaces;
using Domain.Entities; // AppUser için gerekli
using Infrastructure.Data;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        private IEmployeeRepository _employees;
        private IInternRepository _interns;
        private IDepartmentRepository _departments;
        private IUniversityRepository _universities;
        private IInventoryItemRepository _inventoryItems;
        private IAssignmentRepository _assignments;
        private IMaintenanceRepository _maintenances;

        private IGenericRepository<AppUser> _appUsers;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        // Repository Properties
        public IEmployeeRepository Employees => _employees ??= new EmployeeRepository(_context);
        public IInternRepository Interns => _interns ??= new InternRepository(_context);
        public IDepartmentRepository Departments => _departments ??= new DepartmentRepository(_context);
        public IUniversityRepository Universities => _universities ??= new UniversityRepository(_context);
        public IInventoryItemRepository InventoryItems => _inventoryItems ??= new InventoryItemRepository(_context);
        public IAssignmentRepository Assignments => _assignments ??= new AssignmentRepository(_context);
        public IMaintenanceRepository Maintenances => _maintenances ??= new MaintenanceRepository(_context);

        public IGenericRepository<AppUser> AppUsers => _appUsers ??= new GenericRepository<AppUser>(_context);

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}