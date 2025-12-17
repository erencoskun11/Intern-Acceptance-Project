using Application.DTOs.Employee;
using Domain.Entities;
using FluentAssertions;
using MockQueryable;
using MockQueryable.Moq; // MockQueryable burada lazım
using Moq;
using Xunit;

namespace Stajyer_Projesi.Tests.Systems.Services.EmployeeTests
{
    public class GetTests : EmployeeTestBase
    {
        [Fact]
        public async Task GetByIdAsync_ShouldReturnData()
        {
            var employee = _employeeFaker.Generate();
            _mockEmployeeRepo.Setup(x => x.GetByIdAsync(employee.Id)).ReturnsAsync(employee);
            _mockMapper.Setup(x => x.Map<EmployeeListDto>(employee))
                       .Returns(new EmployeeListDto { Id = employee.Id, FirstName = employee.FirstName });

            var result = await _sut.GetByIdAsync(employee.Id);

            result.IsSuccessful.Should().BeTrue();
            result.Data.Id.Should().Be(employee.Id);
        }

        [Fact]
        public async Task GetByDepartmentId_ShouldReturnEmpty_WhenNoData()
        {
            var emptyList = new List<Employee>();
            _mockEmployeeRepo.Setup(x => x.GetByDepartmentId(99))
                .Returns(emptyList.BuildMock()); // MockQueryable

            _mockMapper.Setup(x => x.Map<IEnumerable<EmployeeListDto>>(It.IsAny<List<Employee>>()))
                       .Returns(new List<EmployeeListDto>());

            var result = await _sut.GetByDepartmentIdAsync(99);

            result.Should().BeEmpty();
        }
    }
}