using Application.Constants;
using Application.Services.Interfaces;

namespace Application.Services
{
    public class InventoryCacheManager : IInventoryCacheManager
    {
        private readonly ICacheService _cacheService;

        public InventoryCacheManager(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }
        public void ClearAll()
        {
            _cacheService.Remove(CacheKeys.InventoryItemList);
            _cacheService.Remove(CacheKeys.InventoryAvailableList);
        }
    }
}
