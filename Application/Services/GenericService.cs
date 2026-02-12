using Application.Common;
using Application.Interfaces;
using Application.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class GenericService<TEntity, TListDto, TCreateDto, TUpdateDto> : IGenericService<TListDto, TCreateDto, TUpdateDto>
        where TEntity : class, new()
        where TListDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        protected readonly IGenericRepository<TEntity> _repository;
        protected readonly IMapper _mapper;
        protected readonly IUnitOfWork _uow;

        public GenericService(IGenericRepository<TEntity> repository, IMapper mapper, IUnitOfWork uow)
        {
            _repository = repository;
            _mapper = mapper;
            _uow = uow;
        }

        public virtual async Task<Response<int>> CreateAsync(TCreateDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            await _repository.AddAsync(entity);
            await _uow.CommitAsync();

            var prop = entity.GetType().GetProperty("Id");
            int id = (prop?.GetValue(entity) as int?) ?? 0;

            return Response<int>.Success(id, 201);
        }

        public virtual async Task<Response<NoContent>> UpdateAsync(int id, TUpdateDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return Response<NoContent>.Fail("Kayıt bulunamadı", 404, true);

            _mapper.Map(dto, entity);
            _repository.Update(entity);
            await _uow.CommitAsync();

            return Response<NoContent>.Success(204);
        }

        public virtual async Task<Response<NoContent>> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return Response<NoContent>.Fail("Kayıt bulunamadı", 404, true);

            _repository.Remove(entity);
            await _uow.CommitAsync();

            return Response<NoContent>.Success(204);
        }

        public virtual async Task<Response<IEnumerable<TListDto>>> GetAllAsync()
        {
            var query = _repository.GetAll(); 
            var list = await query.ToListAsync();
            var dtos = _mapper.Map<IEnumerable<TListDto>>(list);
            return Response<IEnumerable<TListDto>>.Success(dtos, 200);
        }

        public virtual async Task<Response<TListDto>> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return Response<TListDto>.Fail("Kayıt bulunamadı", 404, true);

            var dto = _mapper.Map<TListDto>(entity);
            return Response<TListDto>.Success(dto, 200);
        }

    }
}