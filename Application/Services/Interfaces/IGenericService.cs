using Application.Common; 
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IGenericService<TListDto, TCreateDto, TUpdateDto>
    {
        Task<Response<int>> CreateAsync(TCreateDto dto);
        Task<Response<NoContent>> UpdateAsync(int id, TUpdateDto dto);
        Task<Response<NoContent>> DeleteAsync(int id);
        Task<Response<IEnumerable<TListDto>>> GetAllAsync();
        Task<Response<TListDto>> GetByIdAsync(int id);

    }
}