using Application.DTOs.University;

namespace Application.Services.Interfaces
{
    // GenericService kullanımıyla uyumlu olacak şekilde interface güncellendi
    public interface IUniversityService : IGenericService<UniversityListDto, UniversityCreateDto, UniversityUpdateDto>
    {
      
        Task BulkCreateAsync(IEnumerable<UniversityCreateDto> dtos);

    }
}
