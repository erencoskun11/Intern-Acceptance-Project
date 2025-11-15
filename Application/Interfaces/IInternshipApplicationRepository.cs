using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IInternshipApplicationRepository : IGenericRepository<InternshipApplication>
    {
        Task<IEnumerable<InternshipApplication>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<InternshipApplication>> GetByDepartmentIdAsync(int departmentId);
    }
}
