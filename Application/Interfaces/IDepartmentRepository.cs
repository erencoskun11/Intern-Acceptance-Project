using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IDepartmentRepository : IGenericRepository<Department>
    {
        Task<Department?> GetByNameAsync(string name);
    }
}
