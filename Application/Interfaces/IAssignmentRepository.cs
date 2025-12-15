using Domain.Entities;
using System.Linq; 

namespace Application.Interfaces
{
    public interface IAssignmentRepository : IGenericRepository<Assignment>
    {
        IQueryable<Assignment> GetByEmployeeId(int employeeId);

        IQueryable<Assignment> GetByInternId(int internId);
        IQueryable<Assignment> GetActiveAssignmentsByPerson(int? internId, int? employeeId);
    }
}