using Domain.Entities;
using System.Linq; // IQueryable için

namespace Application.Interfaces
{
    public interface IAssignmentRepository : IGenericRepository<Assignment>
    {
        // IQueryable olduğu için "Async" eklerini kaldırdık
        IQueryable<Assignment> GetByEmployeeId(int employeeId);

        IQueryable<Assignment> GetByInternId(int internId);
        IQueryable<Assignment> GetActiveAssignmentsByPerson(int? internId, int? employeeId);
    }
}