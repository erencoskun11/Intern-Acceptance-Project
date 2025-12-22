using Application.Common;
using Application.DTOs.InventoryItem;
using Domain.Entities;

namespace Application.Services.Interfaces
{
    public interface IInventoryItemService : IGenericService<InventoryItemListDto, InventoryItemCreateDto, InventoryItemUpdateDto>
    {
        Task<Response<IEnumerable<InventoryItemListDto>>> GetByCategoryAsync(string category);
        Task<Response<IEnumerable<InventoryItemListDto>>> GetByStatusAsync(string status);
    
    
    
    }
}
