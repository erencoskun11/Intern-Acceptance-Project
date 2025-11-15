using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
        Task<Student?> GetByStudentNumberAsync(string studentNumber);
    }
}
