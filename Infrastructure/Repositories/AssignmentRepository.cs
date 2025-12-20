using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class AssignmentRepository : GenericRepository<Assignment>, IAssignmentRepository
    {
        public AssignmentRepository(AppDbContext context) : base(context)
        {
        }

        public IQueryable<Assignment> GetByEmployeeId(int employeeId)
        {
            return _context.Assignments
                .AsNoTracking()
                .Include(a => a.InventoryItem) 
                .Where(a => a.EmployeeId == employeeId)
                .OrderByDescending(a => a.AssignedAt);
        }

       
        public IQueryable<Assignment> GetByInternId(int internId)
        {
            return _context.Assignments
                .AsNoTracking()
                .Include(a => a.InventoryItem)
                .Where(a => a.InternId == internId)
                .OrderByDescending(a => a.AssignedAt);
        }

        public IQueryable<Assignment> GetActiveAssignmentsByPerson(int? internId, int? employeeId)
        {
            var query = _context.Assignments
                .AsNoTracking()
                .Include(a => a.InventoryItem) 
                .Where(a => a.ActualReturnAt == null); 

            if (internId.HasValue)
                query = query.Where(a => a.InternId == internId);

            if (employeeId.HasValue)
                query = query.Where(a => a.EmployeeId == employeeId);

            return query;
        }
    }
}