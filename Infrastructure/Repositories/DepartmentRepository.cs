using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(AppDbContext ctx) : base(ctx) { }

        public async Task<Department?> GetByNameAsync(string name)
        {
            // GÜVENLİ LINQ: SQL fonksiyonu yerine LINQ kullanıldı. 
            // Name sütununda büyük/küçük harf gözetmeksizin arama yapar.
            return await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Name.ToLower() == name.ToLower());
        }
    }
}