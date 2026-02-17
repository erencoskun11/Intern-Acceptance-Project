using Application.Common;
using Application.DTOs.Maintenance;
using Application.Interfaces;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums; // ItemStatus için
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Application.Services
{
    public class MaintenanceService : GenericService<Maintenance, MaintenanceListDto, MaintenanceCreateDto, MaintenanceUpdateDto>, IMaintenanceService
    {
        private readonly ISignalRService _signalRService;

        public MaintenanceService(IGenericRepository<Maintenance> repository, IMapper mapper, IUnitOfWork uow, ISignalRService signalRService)
            : base(repository, mapper, uow)
        {
            _signalRService = signalRService;
        }

        // --- OKUMA METOTLARI ---
        public async Task<Response<IEnumerable<MaintenanceListDto>>> GetByInventoryItemIdAsync(int itemId)
        {
            var query = _uow.Maintenances.GetByInventoryItemId(itemId);
            var list = await query.ToListAsync();
            var dtos = _mapper.Map<IEnumerable<MaintenanceListDto>>(list);
            return Response<IEnumerable<MaintenanceListDto>>.Success(dtos, 200);
        }

        public async Task<Response<IEnumerable<MaintenanceListDto>>> GetAllWithDetailsAsync()
        {
            var query = _uow.Maintenances.GetAllWithDetails();
            var list = await query.ToListAsync();
            var dtos = _mapper.Map<IEnumerable<MaintenanceListDto>>(list);
            return Response<IEnumerable<MaintenanceListDto>>.Success(dtos, 200);
        }

        // --- 1. CREATE (BAKIM KAYDI AÇMA) ---
        public override async Task<Response<int>> CreateAsync(MaintenanceCreateDto dto)
        {
            // A. Ürünü Bul
            var item = await _uow.InventoryItems.GetByIdAsync(dto.InventoryItemId);
            if (item == null) return Response<int>.Fail("Ürün bulunamadı.", 404, true);

            // B. Zaten bakımda mı kontrolü?
            if (item.Status == ItemStatus.Maintenance)
                return Response<int>.Fail("Bu ürün zaten bakımda.", 400, true);

            // C. Kaydı oluştur
            var maintenance = _mapper.Map<Maintenance>(dto);

            // Eğer DTO'dan gelmiyorsa, Bildirilme tarihini şu an yap
            if (maintenance.ReportedAt == default) maintenance.ReportedAt = DateTime.Now;

            await _uow.Maintenances.AddAsync(maintenance);

            // D. Ürün durumunu "Bakımda" (Maintenance) yap
            item.Status = ItemStatus.Maintenance;
            _uow.InventoryItems.Update(item);

            // E. Kaydet
            await _uow.CommitAsync();

            // --- F. SIGNALR TETİKLEME ---
            await _signalRService.RefreshEntityListAsync("Maintenance");
            await _signalRService.RefreshEntityListAsync("Inventory");
            await _signalRService.RefreshDashboardAsync();
            await _signalRService.SendNotificationAsync($"{item.ItemCode} bakıma alındı.", "warning");

            return Response<int>.Success(maintenance.Id, 201);
        }

        // --- 2. UPDATE ---
        public override async Task<Response<NoContent>> UpdateAsync(int id, MaintenanceUpdateDto dto)
        {
            var response = await base.UpdateAsync(id, dto);

            if (response.IsSuccessful)
            {
                await _signalRService.RefreshEntityListAsync("Maintenance");
            }
            return response;
        }

        // --- 3. DELETE (SİLME) ---
        public override async Task<Response<NoContent>> DeleteAsync(int id)
        {
            // Silmeden önce kaydı bulalım (Ürünü boşa çıkarmak gerekebilir)
            var maintenance = await _uow.Maintenances.GetByIdAsync(id, m => m.InventoryItem);

            if (maintenance == null) return Response<NoContent>.Fail("Kayıt bulunamadı", 404, true);

            // GÜNCELLENDİ: RepairedAt NULL ise (yani bakım bitmemişse) ürünü boşa çıkar
            if (maintenance.InventoryItem != null && maintenance.RepairedAt == null)
            {
                maintenance.InventoryItem.Status = ItemStatus.Available;
                _uow.InventoryItems.Update(maintenance.InventoryItem);
            }

            _uow.Maintenances.Remove(maintenance);
            await _uow.CommitAsync();

            // SignalR Tetikleme
            await _signalRService.RefreshEntityListAsync("Maintenance");
            await _signalRService.RefreshEntityListAsync("Inventory");
            await _signalRService.RefreshDashboardAsync();
            await _signalRService.SendNotificationAsync("Bakım kaydı silindi.", "info");

            return Response<NoContent>.Success(204);
        }

        // --- 4. BAKIMI TAMAMLAMA ---
        public async Task<Response<NoContent>> CompleteMaintenanceAsync(int maintenanceId)
        {
            var maintenance = await _uow.Maintenances.GetByIdAsync(maintenanceId, m => m.InventoryItem);
            if (maintenance == null) return Response<NoContent>.Fail("Kayıt bulunamadı.", 404, true);

            // GÜNCELLENDİ: CompletedAt yerine RepairedAt kullanıyoruz
            if (maintenance.RepairedAt.HasValue)
                return Response<NoContent>.Fail("Bu bakım zaten tamamlanmış.", 400, true);

            maintenance.RepairedAt = DateTime.Now; // Bakım bitti tarihi
            _uow.Maintenances.Update(maintenance);

            // Ürünü tekrar "Müsait" yap
            if (maintenance.InventoryItem != null)
            {
                maintenance.InventoryItem.Status = ItemStatus.Available;
                _uow.InventoryItems.Update(maintenance.InventoryItem);
            }

            await _uow.CommitAsync();

            // Her yeri güncelle
            await _signalRService.RefreshEntityListAsync("Maintenance");
            await _signalRService.RefreshEntityListAsync("Inventory");
            await _signalRService.RefreshDashboardAsync();
            await _signalRService.SendNotificationAsync("Bakım tamamlandı, ürün tekrar müsait.", "success");

            return Response<NoContent>.Success(204);
        }
    }
}