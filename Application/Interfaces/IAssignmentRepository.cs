// IAssignmentRepository.cs
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IAssignmentRepository : IGenericRepository<Assignment>
    {
        Task<IEnumerable<Assignment>> GetByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<Assignment>> GetByStudentIdAsync(int studentId);
    }
}