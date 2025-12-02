using Application.Common;
using Application.DTOs.University;
using Application.Interfaces;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UniversityService : GenericService<University, UniversityListDto, UniversityCreateDto, UniversityUpdateDto>, IUniversityService
    {
        // CS7036 ÇÖZÜMÜ
        public UniversityService(IGenericRepository<University> repository, IMapper mapper, IUnitOfWork uow)
            : base(repository, mapper, uow)
        {
        }

        public async Task<Response<NoContent>> BulkCreateAsync(IEnumerable<UniversityCreateDto> dtos)
        {
            if (dtos == null || !dtos.Any())
                return Response<NoContent>.Fail("Liste boş olamaz.", 400, true);

            var entities = _mapper.Map<List<University>>(dtos);

            foreach (var entity in entities)
            {
                await _repository.AddAsync(entity);
            }

            // GenericRepository'de SaveChanges sildiğimiz için UoW ile kaydediyoruz
            await _uow.CommitAsync();

            return Response<NoContent>.Success(201);
        }
    }
}