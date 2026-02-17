using Application.Common;
using Application.Constants; 
using Application.DTOs.InventoryItem;
using Application.Interfaces;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class InventoryItemService : GenericService<InventoryItem, InventoryItemListDto, InventoryItemCreateDto, InventoryItemUpdateDto>, IInventoryItemService
    {
        private readonly IInventoryItemRepository _repository;
        private readonly ICacheService _cacheService;
        private readonly IInventoryCacheManager _cacheManager;
        private readonly ISignalRService _signalRService;

        public InventoryItemService(
            IInventoryItemRepository repository,
            IMapper mapper,
            IUnitOfWork uow,
            ICacheService cacheService,
            IInventoryCacheManager cacheManager,
            ISignalRService signalRService)
            : base(repository, mapper, uow)
        {
            _repository = repository;
            _cacheService = cacheService;
            _cacheManager = cacheManager;
            _signalRService = signalRService;
        }


        public override async Task<Response<IEnumerable<InventoryItemListDto>>> GetAllAsync()
        {
            if (_cacheService.TryGet(CacheKeys.InventoryItemList, out IEnumerable<InventoryItemListDto> cachedList))
            {
                return Response<IEnumerable<InventoryItemListDto>>.Success(cachedList, 200);
            }

            var response = await base.GetAllAsync();

            if (response.IsSuccessful)
            {
                _cacheService.Set(CacheKeys.InventoryItemList, response.Data, TimeSpan.FromMinutes(30));
            }

            return response;
        }

        public async Task<Response<IEnumerable<InventoryItemListDto>>> GetAvailableItemsAsync()
        {
            if (_cacheService.TryGet(CacheKeys.InventoryAvailableList, out IEnumerable<InventoryItemListDto> cachedList))
            {
                return Response<IEnumerable<InventoryItemListDto>>.Success(cachedList, 200);
            }

            var query = _repository.GetAvailableItems();
            var items = await query.ToListAsync();
            var dtos = _mapper.Map<IEnumerable<InventoryItemListDto>>(items);

            _cacheService.Set(CacheKeys.InventoryAvailableList, dtos, TimeSpan.FromMinutes(30));

            return Response<IEnumerable<InventoryItemListDto>>.Success(dtos, 200);
        }

        public async Task<Response<IEnumerable<InventoryItemListDto>>> GetByStatusAsync(string status)
        {
            if (!Enum.TryParse<ItemStatus>(status, true, out var enumStatus))
                return Response<IEnumerable<InventoryItemListDto>>.Fail("Geçersiz durum (status) değeri.", 400, true);

            var query = _repository.Where(x => x.Status == enumStatus);
            var items = await query.ToListAsync();
            var dtos = _mapper.Map<IEnumerable<InventoryItemListDto>>(items);

            return Response<IEnumerable<InventoryItemListDto>>.Success(dtos, 200);
        }

        public async Task<Response<IEnumerable<InventoryItemListDto>>> GetByCategoryAsync(string category)
        {
            var query = _repository.Where(x => x.Category == category);
            var items = await query.ToListAsync();
            var dtos = _mapper.Map<IEnumerable<InventoryItemListDto>>(items);

            return Response<IEnumerable<InventoryItemListDto>>.Success(dtos, 200);
        }


        public override async Task<Response<int>> CreateAsync(InventoryItemCreateDto dto)
        {
            var exists = await _repository.Where(x => x.ItemCode == dto.ItemCode).AnyAsync();
            if (exists) return Response<int>.Fail($"'{dto.ItemCode}' kodu sistemde zaten kayıtlı.", 400, true);

            var response = await base.CreateAsync(dto);

            if (response.IsSuccessful) 
            {
                _cacheManager.ClearAll(); 
            
                //signalR icin
                await _signalRService.RefreshDashboardAsync();
                await _signalRService.RefreshEntityListAsync("Inventory");
                await _signalRService.SendNotificationAsync("Yeni envanter ürünü eklendi.", "success");

            }
            return response;
        }

        public override async Task<Response<NoContent>> UpdateAsync(int id, InventoryItemUpdateDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return Response<NoContent>.Fail("Ürün bulunamadı.", 404, true);

            var duplicateCheck = await _repository
                .Where(x => x.ItemCode == dto.ItemCode && x.Id != id).AnyAsync();

            if (duplicateCheck) return Response<NoContent>.Fail($"'{dto.ItemCode}' kodu başka bir üründe kullanılıyor.", 400, true);

            _mapper.Map(dto, entity);
            if (string.IsNullOrWhiteSpace(dto.Model)) entity.Model = null;

            _repository.Update(entity);
            await _uow.CommitAsync();

            _cacheManager.ClearAll();

            // SignalR Entegrasyonu
            await _signalRService.RefreshDashboardAsync();
            await _signalRService.RefreshEntityListAsync("Inventory");
            // Güncellemede genellikle bildirim ("toast") atılmaz ama istersen buraya da ekleyebilirsin.


            return Response<NoContent>.Success(204);
        }

        public override async Task<Response<NoContent>> DeleteAsync(int id)
        {
            var response = await base.DeleteAsync(id);
            if (response.IsSuccessful)
            {
                _cacheManager.ClearAll();
            
            await _signalRService.RefreshDashboardAsync();
                await _signalRService.RefreshEntityListAsync("Inventory");
                await _signalRService.SendNotificationAsync("Envanter ürünü silindi.", "warning");

            }
            return response;
        }
    }
}