using Application.DTOs.InventoryItem;
using Application.Interfaces;
using Application.Services;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using MockQueryable;
using MockQueryable.Moq; 
using Moq;

namespace Stajyer_Projesi.Tests
{
    public class InventoryItemTestService
    {
        // --- BU TANIMLAMALAR EKSİKTİ ---
        private readonly Mock<IInventoryItemRepository> _inventoryRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ICacheService> _cacheMock;
        private readonly Mock<IInventoryCacheManager> _cacheManagerMock;

        private readonly InventoryItemService _service; // Test edeceğimiz asıl servis

        // --- BU CONSTRUCTOR (KURUCU) EKSİKTİ ---
        public InventoryItemTestService()
        {
            // 1. Mock'ları oluştur
            _inventoryRepoMock = new Mock<IInventoryItemRepository>();
            _mapperMock = new Mock<IMapper>();
            _uowMock = new Mock<IUnitOfWork>();
            _cacheMock = new Mock<ICacheService>();
            _cacheManagerMock = new Mock<IInventoryCacheManager>();

            // 2. Servisi bu mock'larla başlat
         /*   _service = new InventoryItemService(
                _inventoryRepoMock.Object,
                _mapperMock.Object,
                _uowMock.Object,
                _cacheMock.Object,
                _cacheManagerMock.Object
            );*/
        }

        [Fact]
        public async Task GetAvailableItemsAsync_Should_Return_Only_Available_Items()
        {
            // --- ARRANGE ---

            // Cache boş gibi davran (False dön)
            IEnumerable<InventoryItemListDto> outCache = null;
            _cacheMock.Setup(x => x.TryGet(It.IsAny<string>(), out outCache)).Returns(false);

            var dbData = new List<InventoryItem>
            {
                new InventoryItem { Id = 1, ItemCode = "A1", Status = ItemStatus.Available }, 
                new InventoryItem { Id = 2, ItemCode = "A2", Status = ItemStatus.Assigned },  
                new InventoryItem { Id = 3, ItemCode = "A3", Status = ItemStatus.Maintenance },
                new InventoryItem { Id = 4, ItemCode = "A4", Status = ItemStatus.Available }  
            };

            // MockQueryable ile veritabanı taklidi
            var mockQuery = dbData.AsQueryable().BuildMock();

            // Repository: GetAvailableItems sadece Available olanları (Where) filtreler
            _inventoryRepoMock.Setup(x => x.GetAvailableItems())
                              .Returns(mockQuery.Where(x => x.Status == ItemStatus.Available));

            // Mapper Taklidi
            _mapperMock.Setup(m => m.Map<IEnumerable<InventoryItemListDto>>(It.IsAny<List<InventoryItem>>()))
                       .Returns((List<InventoryItem> src) => src.Select(x => new InventoryItemListDto { ItemCode = x.ItemCode }));

            // --- ACT ---
            var result = await _service.GetAvailableItemsAsync();

            
            result.IsSuccessful.Should().BeTrue();
            result.Data.Should().HaveCount(2); 
            result.Data.All(x => x.ItemCode == "A1" || x.ItemCode == "A4").Should().BeTrue();
        }
    }
}