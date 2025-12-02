using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUniversityRepository : IGenericRepository<University>
    {
        Task<University?> GetByNameAsync(string name);
    }
}
