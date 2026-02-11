using Application.Common;
using Application.Constants; 
using Application.DTOs.Department;
using Application.Interfaces;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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
            var nameExists = await _repository.AnyAsync(x=>x.Name.ToLower() == dto.Name.ToLower());

            if (nameExists)
            {
                return Response<int>.Fail("Bu departman adı zaten mevcut.", 400, true);
            }

            var response = await base.CreateAsync(dto);
            if (response.IsSuccessful) _cacheService.Remove(CacheKeys.DepartmentList);
            return response;
        }


        public override async Task<Response<NoContent>> UpdateAsync(int id, DepartmentUpdateDto dto)
        {
            // Güncelleme yaparken, kendisi hariç (x.Id != id) aynı isimde başka kayıt var mı?
            var nameExists = await _repository.AnyAsync(x => x.Name.ToLower() == dto.Name.ToLower() && x.Id != id);

            if (nameExists)
            {
                return Response<NoContent>.Fail("Bu isimde başka bir departman zaten mevcut.", 400, true);
            }

            var response = await base.UpdateAsync(id, dto);
            if (response.IsSuccessful) _cacheService.Remove(CacheKeys.DepartmentList);
            return response;
        }

        public override async Task<Response<NoContent>> DeleteAsync(int id)
        { 
       try
            {
                // 1. Base metodu çağırmak yerine işlemi kendimiz yapıyoruz ki hatayı yakalayabilelim.
                // GenericService içinde _repository ve _uow genellikle 'protected' tanımlıdır, erişebiliriz.
                
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    return Response<NoContent>.Fail("Kayıt bulunamadı.", 404, true);
                }

    _repository.Remove(entity);
                
                // Hata tam burada, veritabanına kaydederken fırlar
                await _uow.CommitAsync();

    // 2. İşlem başarılıysa önbelleği temizle
    _cacheService.Remove(CacheKeys.DepartmentList);

                return Response<NoContent>.Success(204);
            }
            catch (DbUpdateException ex)
            {
                // 3. PostgreSQL özel hatasını (Foreign Key Violation - 23503) yakala
                if (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23503")
                {
                    return Response<NoContent>.Fail("Bu departmanı silemezsiniz çünkü içerisinde kayıtlı personeller var.", 400, true);
                }

                // Başka bir hataysa fırlatmaya devam et (500 Hatası olsun)
                throw;
            }
        }
        
    }
}