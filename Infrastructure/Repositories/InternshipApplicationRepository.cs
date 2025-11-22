using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class InternshipApplicationRepository : GenericRepository<InternshipApplication>, IInternshipApplicationRepository
    {
        public InternshipApplicationRepository(AppDbContext ctx) : base(ctx) { }

        public async Task<IEnumerable<InternshipApplication>> GetByStudentIdAsync(int studentId)
        {
            return await _dbSet.AsNoTracking()
                               .Where(a => a.InternId == studentId)
                               .Include(a => a.Intern)
                               .Include(a => a.Department)
                               .Include(a => a.Employee)
                               .ToListAsync();
        }

        public async Task<IEnumerable<InternshipApplication>> GetByDepartmentIdAsync(int departmentId)
        {
            return await _dbSet.AsNoTracking()
                               .Where(a => a.DepartmentId == departmentId)
                               .Include(a => a.Intern)
                               .Include(a => a.Employee)
                               .ToListAsync();
        }

       

      
    }
}
