using Application.Common; // Response için
using Application.DTOs.Maintenance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IMaintenanceService : IGenericService<MaintenanceListDto, MaintenanceCreateDto, MaintenanceUpdateDto>
    {
        // Hata Çözümü: Dönüş tipleri Task<Response<...>> olmalı
        Task<Response<IEnumerable<MaintenanceListDto>>> GetByInventoryItemIdAsync(int itemId);

        Task<Response<NoContent>> CompleteMaintenanceAsync(int maintenanceId);
        Task<Response<IEnumerable<MaintenanceListDto>>> GetAllWithDetailsAsync();
    }
}