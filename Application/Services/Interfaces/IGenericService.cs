using Application.Common; // Response burada
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IGenericService<TListDto, TCreateDto, TUpdateDto>
    {
        // Dönüş tiplerini Response<T> yaptık
        Task<Response<int>> CreateAsync(TCreateDto dto);
        Task<Response<NoContent>> UpdateAsync(int id, TUpdateDto dto);
        Task<Response<NoContent>> DeleteAsync(int id);
        Task<Response<IEnumerable<TListDto>>> GetAllAsync();
        Task<Response<TListDto>> GetByIdAsync(int id);
    }
}
