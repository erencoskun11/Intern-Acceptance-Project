using Application.DTOs.InventoryItem;
using Application.Interfaces;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class InventoryItemService : IInventoryItemService
    {
        private readonly IInventoryItemRepository _repository;
        private readonly IMapper _mapper;

        public InventoryItemService(IInventoryItemRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // Generic CRUD metotları

        public async Task<int> CreateAsync(InventoryItemCreateDto dto)
        {
            var entity = _mapper.Map<InventoryItem>(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return entity.Id;
        }

        public async Task UpdateAsync(int id, InventoryItemUpdateDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException("InventoryItem not found");

            _mapper.Map(dto, entity); // dto değerlerini entity'ye uygula
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
            await _repository.SaveChangesAsync();
        }

        public async Task<InventoryItemListDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<InventoryItemListDto>(entity);
        }

        public async Task<IEnumerable<InventoryItemListDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<InventoryItemListDto>>(entities);
        }

        public async Task<IEnumerable<InventoryItemListDto>> GetByStatusAsync(string status)
        {
            if (!Enum.TryParse<ItemStatus>(status, out var enumStatus))
                throw new ArgumentException("Invalid status value");

            var entities = await _repository.FindAsync(x => x.Status == enumStatus);
            return _mapper.Map<IEnumerable<InventoryItemListDto>>(entities);
        }

    }
}
