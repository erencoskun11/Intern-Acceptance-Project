using Application.DTOs.Assignment;
using Application.Interfaces; // For IUnitOfWork
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions; // For readable assertions
using MockQueryable;
using MockQueryable.Moq; // You might need this for IQueryable mocking, or see the simpler approach below
using Moq; // For faking the database
using System.Linq.Expressions;

namespace Stajyer_Projesi.Tests
{
    public class AssignmentServiceTests
    {
        // 1. Define the Mocks (The Fake Objects)
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AssignmentService _service; // The real service we are testing

        public AssignmentServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            // Inject the fakes into the real service
          //  _service = new AssignmentService(_uowMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Fail_When_User_Already_Has_Item_In_Same_Category()
        {
            // --- ARRANGE (Preparation) ---

            // 1. Setup the input DTO
            var dto = new AssignmentCreateDto
            {
                InventoryItemId = 100,
                InternId = 5,
                EmployeeId = null
            };

            // 2. Setup the "Item to be assigned" (A Laptop)
            var newItem = new InventoryItem
            {
                Id = 100,
                Category = "Laptop",
                Status = ItemStatus.Available
            };

            // 3. Setup the "Existing Assignment" (User already has a Laptop)
            var existingAssignments = new List<Assignment>
            {
                new Assignment
                {
                    Id = 1,
                    InternId = 5,
                    InventoryItem = new InventoryItem { Category = "Laptop" } // Same Category!
                }
            };

            // 4. Teach the Mock UoW how to behave
            // When GetByIdAsync(100) is called, return 'newItem'
            _uowMock.Setup(x => x.InventoryItems.GetByIdAsync(100))
                    .ReturnsAsync(newItem);

            // When GetActiveAssignments... is called, return the list with the existing laptop
            // Note: Since our Repo returns IQueryable, we need to mock that list as AsyncQueryable.
            // A simple trick is to return a list wrapped in a Mock helper or verify logic differently.
            // For now, let's assume we mocked the repository method to return the list.

            // *To make IQueryable mocking easy, install 'MockQueryable.Moq' from NuGet*
            var mockQuery = existingAssignments.AsQueryable().BuildMock();

            _uowMock.Setup(x => x.Assignments.GetActiveAssignmentsByPerson(5, null))
                    .Returns(mockQuery);

            // --- ACT (Execution) ---
            var result = await _service.CreateAsync(dto);

            // --- ASSERT (Verification) ---

            // The result should not be successful
            result.IsSuccessful.Should().BeFalse();

            // The error message should mention the category rule
            result.Error.Errors.Should().Contain(e => e.Contains("zaten 'Laptop' kategorisinde"));

            // Ensure we NEVER called AddAsync (Database save should not happen)
            _uowMock.Verify(x => x.Assignments.AddAsync(It.IsAny<Assignment>()), Times.Never);
        }
        [Fact]
        public async Task AssignAsync_Should_Return_Success_When_Conditions_Are_Valid()
        {
            // ARRANGE
            var dto = new AssignmentCreateDto { InventoryItemId = 200, InternId = 5 };

            // Müsait bir ürün
            var item = new InventoryItem { Id = 200, Category = "Laptop", Status = ItemStatus.Available };

            // Kullanıcının daha önce hiç zimmeti yok (Boş liste)
            var emptyAssignments = new List<Assignment>();

            // Mock ayarları
            _uowMock.Setup(u => u.InventoryItems.GetByIdAsync(200)).ReturnsAsync(item);

            // Boş liste dönmesi için MockQueryable
            var mockQuery = emptyAssignments.AsQueryable().BuildMock();
            _uowMock.Setup(u => u.Assignments.GetActiveAssignmentsByPerson(5, null)).Returns(mockQuery);

            _mapperMock.Setup(m => m.Map<Assignment>(dto)).Returns(new Assignment { Id = 99 });

            // ACT
            var result = await _service.CreateAsync(dto);

            // ASSERT
            result.IsSuccessful.Should().BeTrue();

            // Ürün durumu "Assigned" oldu mu?
            item.Status.Should().Be(ItemStatus.Assigned);

            // Kaydetme metodu çağrıldı mı?
            _uowMock.Verify(u => u.CommitAsync(), Times.Once);
        }
        [Fact]
        public async Task AssignAsync_Should_Fail_When_Item_Is_In_Maintenance()
        {
            // ARRANGE
            var dto = new AssignmentCreateDto { InventoryItemId = 300, InternId = 5 };

            // Bakımda olan bir ürün
            var item = new InventoryItem { Id = 300, Status = ItemStatus.Maintenance };

            _uowMock.Setup(u => u.InventoryItems.GetByIdAsync(300)).ReturnsAsync(item);

            // ACT
            var result = await _service.CreateAsync(dto);

            // ASSERT
            result.IsSuccessful.Should().BeFalse();
            result.Error.Errors[0].Should().Contain("zimmetlenemez"); // Hata mesajında bu kelime geçmeli

            // Commit ASLA çağrılmamalı
            _uowMock.Verify(u => u.CommitAsync(), Times.Never);
        }


        [Fact]
        public async Task AssignAsync_Should_Fail_When_Item_Not_Found()
        {
            // ARRANGE
            var dto = new AssignmentCreateDto { InventoryItemId = 999 }; // Olmayan ID

            // Null dön (Bulunamadı)
            _uowMock.Setup(u => u.InventoryItems.GetByIdAsync(999)).ReturnsAsync((InventoryItem)null);

            // ACT
            var result = await _service.CreateAsync(dto);

            // ASSERT
            result.IsSuccessful.Should().BeFalse();
            result.Error.Errors[0].Should().Contain("bulunamadı");
        }


        [Fact]
        public async Task ReturnAsync_Should_Update_Assignment_And_Make_Item_Available()
        {
            // --- ARRANGE (Hazırlık) ---
            var returnDto = new AssignmentReturnDto
            {
                AssignmentId = 10,
                ActualReturnAt = DateTime.UtcNow
            };

            // Mevcut Zimmet Kaydı (Hala aktif, iade edilmemiş)
            var assignment = new Assignment
            {
                Id = 10,
                InventoryItemId = 50,
                ActualReturnAt = null,
                InventoryItem = new InventoryItem
                {
                    Id = 50,
                    Status = ItemStatus.Assigned // Şu an zimmetli görünüyor
                }
            };

            // Mock: GetById çağrıldığında bu zimmeti ve ürünü getir
            _uowMock.Setup(u => u.Assignments.GetByIdAsync(10, It.IsAny<Expression<Func<Assignment, object>>[]>()))
                    .ReturnsAsync(assignment);

            // Mock: Ürünü de ayrıca çekebilelim
            _uowMock.Setup(u => u.InventoryItems.GetByIdAsync(50, null))
                    .ReturnsAsync(assignment.InventoryItem);

            // --- ACT (Eylem) ---
            var result = await _service.ReturnAsync(returnDto);

            // --- ASSERT (Kontrol) ---
            result.IsSuccessful.Should().BeTrue(); // İşlem başarılı mı?

            // 1. Kontrol: Zimmetin iade tarihi doldu mu?
            assignment.ActualReturnAt.Should().NotBeNull();

            // 2. Kontrol: Ürün durumu "Assigned" -> "Available" oldu mu?
            assignment.InventoryItem.Status.Should().Be(ItemStatus.Available);

            // 3. Kontrol: Veritabanına "Güncelle" ve "Kaydet" komutları gitti mi?
            _uowMock.Verify(u => u.Assignments.Update(assignment), Times.Once);
            _uowMock.Verify(u => u.InventoryItems.Update(assignment.InventoryItem), Times.Once);
            _uowMock.Verify(u => u.CommitAsync(), Times.Once);
        }
































    }
}