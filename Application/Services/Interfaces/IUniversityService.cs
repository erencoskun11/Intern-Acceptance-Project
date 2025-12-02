using Application.Common;
using Application.DTOs.University;

namespace Application.Services.Interfaces
{
    // GenericService kullanımıyla uyumlu olacak şekilde interface güncellendi
    public interface IUniversityService : IGenericService<UniversityListDto, UniversityCreateDto, UniversityUpdateDto>
    {
      
        Task <Response<NoContent>>BulkCreateAsync(IEnumerable<UniversityCreateDto> dtos);

    }
}
