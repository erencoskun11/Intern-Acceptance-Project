using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using MockQueryable;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stajyer_Projesi.Tests.Systems.Services.AssignmentTests
{
    public class CreateTests : AssignmentTestBase
    {
        [Fact]
        public async Task Create_ShouldReturnSuccess_WhenEverythingIsValid()
        {
            // Arrange
            var dto = _createFaker.Generate();
            dto.InternId = 5;
            dto.EmployeeId = null;

            //1.urun musait
            var item = new InventoryItem { Id = dto.InventoryItemId, Category = "Laptop", Status = ItemStatus.Available };
            _mockItemRepo.Setup(x => x.GetByIdAsync(dto.InventoryItemId)).ReturnsAsync(item);


            //2.kullanıcının aynı kategoride ürünü yok
            var emptyList = new List<Assignment>();
            _mockAssignmentRepo.Setup(x=>x.GetActiveAssignmentsByPerson(dto.InternId,null))
                .Returns(emptyList.BuildMock());


            //3.mapper ayarı
            _mockMapper.Setup(x=>x.Map<Assignment>(dto)).Returns(new Assignment 
            { 
                Id=100,
               
            });

            // Act
            var result = await _sut.CreateAsync(dto);

            // Assert
            result.IsSuccessful.Should().BeTrue();
            result.StatusCode.Should().Be(201);

            //urun durumu "available" -> "assigned" oldumu
            item.Status.Should().Be(ItemStatus.Assigned);
            _mockItemRepo.Verify(x => x.Update(item), Times.Once);

            //kaydedildimi
            _mockAssignmentRepo.Verify(x => x.AddAsync(It.IsAny<Assignment>()), Times.Once);
            _mockUow.Verify(x => x.CommitAsync(), Times.Once);

            //signalR gitti mi?
            _mockSignalR.Verify(x => x.SendNotificationAsync(It.IsAny<string>(), "success"), Times.Once);
        }
        [Fact]
        public async Task CreateAsync_ShouldFail_WhenItemNotFound()
        {
            // Arrange
            var dto = _createFaker.Generate();
            _mockItemRepo.Setup(x => x.GetByIdAsync(dto.InventoryItemId)).ReturnsAsync((InventoryItem?)null);

            // Act
            var result = await _sut.CreateAsync(dto);

            // Assert
            result.IsSuccessful.Should().BeFalse();
            result.Error.Errors.First().Should().Contain("bulunamadı");
        }


        [Fact]
        public async Task CreateAsync_ShouldFail_WhenItemIsNotAvailable()
        {
            //ARRANGE
            var dto = _createFaker.Generate();
            //1.urun musait degil
            var item = new InventoryItem { Id = dto.InventoryItemId, Status = ItemStatus.Assigned };
            _mockItemRepo.Setup(x => x.GetByIdAsync(dto.InventoryItemId)).ReturnsAsync(item);

            //ACT
            var result = await _sut.CreateAsync(dto);

            //ASSERT
            result.IsSuccessful.Should().BeFalse();
            result.Error.Errors.First().Should().Contain("zimmetlenemez");





        }

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenUserHasSameCategoryItem()
        {
            // --- ARRANGE ---
            var dto = _createFaker.Generate();
            dto.InternId = 1;

            // Alınmak istenen ürün: Laptop
            var newItem = new InventoryItem { Id = dto.InventoryItemId, Category = "Laptop", Status = ItemStatus.Available };
            _mockItemRepo.Setup(x => x.GetByIdAsync(dto.InventoryItemId)).ReturnsAsync(newItem);

            // Kullanıcının ZATEN bir Laptop'u var
            var existingAssignments = new List<Assignment>
            {
                new Assignment
                {
                    Id = 50,
                    InventoryItem = new InventoryItem { Category = "Laptop" } // Aynı kategori!
                }
            };

            // Repo bu listeyi dönsün
            _mockAssignmentRepo.Setup(x => x.GetActiveAssignmentsByPerson(dto.InternId, null))
                .Returns(existingAssignments.BuildMock());

            // --- ACT ---
            var result = await _sut.CreateAsync(dto);

            // --- ASSERT ---
            result.IsSuccessful.Should().BeFalse();
            result.Error.Errors.First().Should().Contain("zaten 'Laptop' kategorisinde");

            // Veritabanına ASLA kayıt atılmamalı
            _mockUow.Verify(x => x.CommitAsync(), Times.Never);
        }











    }
}
