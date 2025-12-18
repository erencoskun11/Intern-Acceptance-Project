using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Npgsql;
using Xunit;

namespace Stajyer_Projesi.Tests.Systems.Services.DepartmentTests
{
    public class DeleteTests : DepartmentTestBase
    {
        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenIdExists()
        {
            // --- ARRANGE ---
            var dept = _deptFaker.Generate();
            _mockRepo.Setup(x => x.GetByIdAsync(dept.Id)).ReturnsAsync(dept);

            // --- ACT ---
            var result = await _sut.DeleteAsync(dept.Id);

            // --- ASSERT ---
            result.IsSuccessful.Should().BeTrue();

            _mockRepo.Verify(x => x.Remove(dept), Times.Once);
            _mockUow.Verify(x => x.CommitAsync(), Times.Once);

            // Başarılı silme sonrası cache temizlenmeli
            _mockCache.Verify(x => x.Remove(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnCustomError_WhenEmployeesExist()
        {
            // --- ARRANGE ---
            var dept = _deptFaker.Generate();
            _mockRepo.Setup(x => x.GetByIdAsync(dept.Id)).ReturnsAsync(dept);

            // Postgres Hatası Simülasyonu (23503 kodu)
            var postgresEx = new PostgresException("foreign key violation", "ERROR", "ERROR", "23503");
            var dbUpdateEx = new DbUpdateException("DB Error", postgresEx);

            // Commit yaparken bu hata fırlasın
            _mockUow.Setup(x => x.CommitAsync()).ThrowsAsync(dbUpdateEx);

            // --- ACT ---
            var result = await _sut.DeleteAsync(dept.Id);

            // --- ASSERT ---
            // Senin kodunda try-catch ile yakalanıp 400 dönülüyor
            result.IsSuccessful.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Error.Errors.First().Should().Contain("silemezsiniz çünkü içerisinde kayıtlı personeller var");
        }
    }
}