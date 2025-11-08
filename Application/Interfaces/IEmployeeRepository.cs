// IEmployeeRepository.cs
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<Employee?> GetByEmailAsync(string email);
        Task<IEnumerable<Employee>> GetByDepartmentIdAsync(int departmentId);
    }
}

