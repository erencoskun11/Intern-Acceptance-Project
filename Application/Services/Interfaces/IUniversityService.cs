using Application.Common;
using Application.DTOs.University;

namespace Application.Services.Interfaces
{
    public interface IUniversityService : IGenericService<UniversityListDto, UniversityCreateDto, UniversityUpdateDto>
    {
      
        Task <Response<NoContent>>BulkCreateAsync(IEnumerable<UniversityCreateDto> dtos);

    }
}
