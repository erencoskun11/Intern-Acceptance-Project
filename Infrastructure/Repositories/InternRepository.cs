using Application.DTOs.Intern;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class InternRepository : GenericRepository<Intern>, IInternRepository
    {
        public InternRepository(AppDbContext context) : base(context) { }

        public IQueryable<Intern> GetAllWithDetails()
        {
            return _context.Interns
                .Include(s => s.University) 
                .AsNoTracking();
        }

        public async Task<InternResultDto> CreateWithValidationAsync(InternCreateDto dto)
        {
            // Context kullanımı sadece burada (Infrastructure katmanında) kalır
            return await _context.Database
                .SqlQueryRaw<InternResultDto>(
                    "SELECT * FROM CreateInternWithValidation({0}, {1}, {2}, {3}, {4}, {5})",
                    dto.FirstName, dto.LastName, dto.Email, dto.Phone, dto.DepartmentId, dto.UniversityId)
                .FirstOrDefaultAsync();
        }
        public async Task<Intern> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}