using Application.DTOs.Employee;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Stajyer_Projesi.Tests.Systems.Services.EmployeeTests
{
    // Base sınıftan miras alıyoruz!
    public class CreateTests : EmployeeTestBase
    {
        [Fact]
        public async Task CreateAsync_ShouldReturnSuccess_WhenUnique()
        {
            var createDto = _createDtoFaker.Generate();
            _mockEmployeeRepo.Setup(x => x.GetByEmailAsync(createDto.Email)).ReturnsAsync((Employee)null);
            _mockEmployeeRepo.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Employee, bool>>>())).ReturnsAsync(false);

            _mockMapper.Setup(x => x.Map<Employee>(createDto)).Returns(new Employee());

            var result = await _sut.CreateAsync(createDto);

            result.IsSuccessful.Should().BeTrue();
            _mockEmployeeRepo.Verify(x => x.AddAsync(It.IsAny<Employee>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenEmailExists()
        {
            var createDto = _createDtoFaker.Generate();
            _mockEmployeeRepo.Setup(x => x.GetByEmailAsync(createDto.Email))
                .ReturnsAsync(new Employee()); // Dolu dön

            var result = await _sut.CreateAsync(createDto);

            result.IsSuccessful.Should().BeFalse();
            result.Error.Errors.First().Should().Contain("gmail");
        }

        // Diğer Create testleri (PhoneExists, Exception vb.) buraya...
    }
}