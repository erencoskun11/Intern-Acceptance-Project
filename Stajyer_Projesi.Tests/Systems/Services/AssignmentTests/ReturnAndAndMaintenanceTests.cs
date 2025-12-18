using Application.DTOs.Assignment;
using Application.DTOs.Maintenance;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Stajyer_Projesi.Tests.Systems.Services.AssignmentTests
{
    public class ReturnAndAndMaintenanceTests : AssignmentTestBase
    {
        [Fact]
        public async Task ReturnAsync_ShouldSuccess_And_MakeItemAvailable()
        {
            //Arrange
            var returnDto = new AssignmentReturnDto { AssignmentId = 1 };

            //Iade edilen zimmet
            // İade edilecek zimmet
            var assignment = new Assignment
            {
                Id = 1,
                InventoryItem = new InventoryItem { Id = 10, Status = ItemStatus.Assigned },
                ActualReturnAt = null // Henüz iade edilmemiş
            };
            // Include edilen InventoryItem ile birlikte dönsün
            _mockAssignmentRepo.Setup(x => x.GetByIdAsync(1, It.IsAny<Expression<Func<Assignment, object>>[]>())).ReturnsAsync(assignment);
                        //Act
            var result = await _sut.ReturnAsync(returnDto);
                        //Assert
            result.IsSuccessful.Should().BeTrue();
            //Tarih duldu mu ?
            assignment.ActualReturnAt.Should().NotBeNull();

            //Envanter durumu Available mı oldu ?
            assignment.InventoryItem.Status.Should().Be(ItemStatus.Available);

            _mockUow.Verify(x=>x.CommitAsync(), Times.Once);    

        }

        [Fact]
        public async Task ReturnAsync_ShouldFail_IfAlreadyReturned()
        {
            //ARRANGE
            var assignment = new Assignment
            { Id = 1, ActualReturnAt = DateTime.UtcNow.AddDays(-1) };

            _mockAssignmentRepo.Setup(x => x.GetByIdAsync(1, It.IsAny<Expression<Func<Assignment, object>>[]>()))
                .ReturnsAsync(assignment);

            //ACT
            var result = await _sut.ReturnAsync(new AssignmentReturnDto { AssignmentId = 1 });

            //ASSERT
            result.IsSuccessful.Should().BeFalse();
            result.Error.Errors.Should().Contain("zaten iade edilmiş ");



        }

        [Fact]
        public async Task CreateMaintenanceAsync_ShouldSuccess_WhenItemIsValid()
        {
            // --- ARRANGE ---
            var dto = new MaintenanceCreateDto { InventoryItemId = 5 };

            // Ürün bozuk değil, zimmetli değil (Available)
            var item = new InventoryItem { Id = 5, Status = ItemStatus.Available };

            _mockItemRepo.Setup(x => x.GetByIdAsync(5)).ReturnsAsync(item);
            _mockMapper.Setup(x => x.Map<Maintenance>(dto)).Returns(new Maintenance());

            // --- ACT ---
            var result = await _sut.CreateMaintenanceAsync(dto);

            // --- ASSERT ---
            result.IsSuccessful.Should().BeTrue();

            // Ürün durumu "Maintenance" oldu mu?
            item.Status.Should().Be(ItemStatus.Maintenance);

            _mockMaintenanceRepo.Verify(x => x.AddAsync(It.IsAny<Maintenance>()), Times.Once);
            _mockUow.Verify(x => x.CommitAsync(), Times.Once);
        }

    }
}
