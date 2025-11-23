using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UniversityRepository : GenericRepository<University>, IUniversityRepository
    {
        public UniversityRepository(AppDbContext ctx) : base(ctx) { }

        public async Task<University?> GetByNameAsync(string name)
        {
            return await _dbSet.AsNoTracking()
                               .FirstOrDefaultAsync(u => u.Name.ToLower() == name.ToLower());
        }
    }
}
