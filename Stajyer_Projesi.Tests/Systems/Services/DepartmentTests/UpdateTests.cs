using Application.DTOs.Department;
using Domain.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stajyer_Projesi.Tests.Systems.Services.DepartmentTests
{
    public class UpdateTests : DepartmentTestBase
    {
        [Fact]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenIdExists()
        {
            //Arrange
            var dept = _deptFaker.Generate();
            var updateDto = new DepartmentUpdateDto { Name = "Yeni İsim"};

            //Id bulundu 
            _mockRepo.Setup(x => x.GetByIdAsync(dept.Id))
                .ReturnsAsync(dept);

            //commit basarili
            _mockUow.Setup(x => x.CommitAsync())
                .ReturnsAsync(1);

            //Act
            var result = await _sut.UpdateAsync(dept.Id, updateDto);

            //Assert
            result.IsSuccessful.Should().BeTrue();
            result.StatusCode.Should().Be(204);


            _mockCache.Verify(x => x.Remove(It.IsAny<string>()), Times.Once);
        }
        [Fact]
        public async Task UpdateAsync_ShouldReturnFail_WhenIdDoesNotExist()
        {
            //Arrange
            int nonExistentId = 99;
            var updateDto = new DepartmentUpdateDto { Name = "Test" };

            //Kayit yok(null)
            _mockRepo.Setup(x => x.GetByIdAsync(nonExistentId)).ReturnsAsync((Department)null);

            //Act
            var result = await _sut.UpdateAsync(nonExistentId,updateDto);

            //Assert
            result.IsSuccessful.Should().BeFalse();
            result.StatusCode.Should().Be(404);

            _mockUow.Verify(x => x.CommitAsync(), Times.Never);
        }



    }
}
