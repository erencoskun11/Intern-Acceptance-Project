using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;
using Npgsql;
using Microsoft.EntityFrameworkCore;

namespace Stajyer_Projesi.Tests.Systems.Services.EmployeeTests
{
    public class DeleteTests : EmployeeTestBase
    {
        [Fact]
        public async Task DeleteAsync_ShouldSuccess_WhenIdExists()
        {
            var employee = _employeeFaker.Generate();
            _mockEmployeeRepo.Setup(x => x.GetByIdAsync(employee.Id)).ReturnsAsync(employee);

            var result = await _sut.DeleteAsync(employee.Id);

            result.IsSuccessful.Should().BeTrue();
            _mockUow.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldFail_WhenIdDoesNotExist()
        {
            _mockEmployeeRepo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Employee)null);

            var result = await _sut.DeleteAsync(999);

            result.IsSuccessful.Should().BeFalse();
            result.StatusCode.Should().Be(404);
        }

        // Zimmet (Foreign Key) testi buraya...
    }
}