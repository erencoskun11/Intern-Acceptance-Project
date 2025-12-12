using Application.DTOs.Employee;
using Domain.Entities;

namespace Application.Services.Interfaces
{
    public interface IEmployeeService : IGenericService<EmployeeListDto, EmployeeCreateDto, EmployeeUpdateDto>
    {
        // Opsiyonel: özel metotlar eklenebilir
        Task<EmployeeListDto?> GetByEmailAsync(string email);

        Task<IEnumerable<EmployeeListDto>> GetByDepartmentIdAsync(int departmentId);
    }
}