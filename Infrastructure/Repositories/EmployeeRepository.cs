using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(AppDbContext context) : base(context)
        {
        }

        // 1. MAİL ADRESİNE GÖRE GETİR (LINQ)
        public async Task<Employee?> GetByEmailAsync(string email)
        {
            // GÜVENLİ LINQ: SQL fonksiyonu yerine LINQ kullanıldı.
            return await _context.Employees
                .Include(e => e.Department) // Departman bilgisini de dahil et
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Email.ToLower() == email.ToLower());
        }

        // 2. DEPARTMANA GÖRE GETİR (LINQ)
        public IQueryable<Employee> GetByDepartmentId(int departmentId)
        {
            // GÜVENLİ LINQ: SQL fonksiyonu yerine LINQ kullanıldı.
            return _context.Employees
                .Include(e => e.Department)
                .Where(e => e.DepartmentId == departmentId)
                .AsNoTracking();
        }
    }
}