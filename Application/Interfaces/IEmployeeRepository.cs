// IEmployeeRepository.cs
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<Employee?> GetByEmailAsync(string email);
        IQueryable<Employee> GetByDepartmentId(int departmentId);
    }
}

