using Application.Common;
using Application.Constants;
using Application.DTOs.University;
using Application.Interfaces;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;


namespace Application.Services
{
    public class UniversityService : GenericService<University, UniversityListDto, UniversityCreateDto, UniversityUpdateDto>, IUniversityService
    {
        ICacheService _cacheService;
        public UniversityService(IGenericRepository<University> repository, IMapper mapper, IUnitOfWork uow,ICacheService cacheService)
            : base(repository, mapper, uow)
        {
            _cacheService = cacheService;
        }

        public override async Task<Response<IEnumerable<UniversityListDto>>> GetAllAsync()
        {
            if(_cacheService.TryGet(CacheKeys.UniversityList, out IEnumerable<UniversityListDto> cachedList))
            {
                return Response<IEnumerable<UniversityListDto>>.Success(cachedList, 200);
            }

            var response = await base.GetAllAsync();

            if (response.IsSuccessful)
            {
                _cacheService.Set(CacheKeys.UniversityList, response.Data, TimeSpan.FromMinutes(30));
            }
            return response;
        }
        public override async Task<Response<int>> CreateAsync(UniversityCreateDto dto)
        {
           var response = await base.CreateAsync(dto);
            if (response.IsSuccessful)
            {
                _cacheService.Remove(CacheKeys.UniversityList);
            }
            return response;
        }

        public override async Task<Response<NoContent>> UpdateAsync(int id ,UniversityUpdateDto dto)
        {
            var response = await base.UpdateAsync(id, dto);
            if (response.IsSuccessful)_cacheService.Remove(CacheKeys.UniversityList);
            return response;
        }
        public override async Task<Response<NoContent>> DeleteAsync(int id)
        {
            var response = await base.DeleteAsync(id);
            if (response.IsSuccessful) _cacheService.Remove(CacheKeys.UniversityList);
            return response;
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

            await _uow.CommitAsync();

            return Response<NoContent>.Success(201);
        }
    }
}