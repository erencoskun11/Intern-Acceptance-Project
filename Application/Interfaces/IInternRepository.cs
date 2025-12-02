using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IInternRepository : IGenericRepository<Intern>
    {
        IQueryable<Intern> GetAllWithDetails();
    }
}
