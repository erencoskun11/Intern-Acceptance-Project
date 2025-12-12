using Application.Common;
using Application.Constants; 
using Application.DTOs.Department;
using Application.Interfaces;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
namespace Application.Services
{
    public class DepartmentService : GenericService<Department, DepartmentListDto, DepartmentCreateDto, DepartmentUpdateDto>, IDepartmentService
    {
        private readonly ICacheService _cacheService;

        public DepartmentService(
            IGenericRepository<Department> repository,
            IMapper mapper,
            IUnitOfWork uow,
            ICacheService cacheService)
            : base(repository, mapper, uow)
        {
            _cacheService = cacheService;
        }

        public override async Task<Response<IEnumerable<DepartmentListDto>>> GetAllAsync()
        {
            if (_cacheService.TryGet(CacheKeys.DepartmentList, out IEnumerable<DepartmentListDto> cachedList))
            {
                return Response<IEnumerable<DepartmentListDto>>.Success(cachedList, 200);
            }

            var response = await base.GetAllAsync();

            if (response.IsSuccessful)
            {
                _cacheService.Set(CacheKeys.DepartmentList, response.Data, TimeSpan.FromMinutes(30));
            }

            return response;
        }
       

        public override async Task<Response<int>> CreateAsync(DepartmentCreateDto dto)
        {
            var response = await base.CreateAsync(dto);
            if (response.IsSuccessful) _cacheService.Remove(CacheKeys.DepartmentList);
            return response;
        }

       
        public override async Task<Response<NoContent>> UpdateAsync(int id, DepartmentUpdateDto dto)
        {
            var response = await base.UpdateAsync(id, dto);
            if (response.IsSuccessful) _cacheService.Remove(CacheKeys.DepartmentList);
            return response;
        }

        public override async Task<Response<NoContent>> DeleteAsync(int id)
        {
            var response = await base.DeleteAsync(id);
            if (response.IsSuccessful) _cacheService.Remove(CacheKeys.DepartmentList);
            return response;
        }
    }
}