using Application.DTOs.Employee;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Stajyer_Projesi.Tests.Systems.Services.EmployeeTests
{
    public class UpdateTests : EmployeeTestBase
    {
        [Fact]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenIdExists()
        {
            // --- ARRANGE ---
            // 1. Var olan bir çalışan üret
            var employee = _employeeFaker.Generate();

            // 2. Güncelleme verisi hazırla
            var updateDto = new EmployeeUpdateDto
            {
                FirstName = "Güncellenen İsim",
                LastName = "Güncellenen Soyad",
                Phone = "5551112233",
                Email = employee.Email // Mail değişmiyor varsayalım
            };

            // 3. Repository: Bu ID sorulduğunda çalışanı dön
            _mockEmployeeRepo.Setup(x => x.GetByIdAsync(employee.Id))
                .ReturnsAsync(employee);

            // 4. Commit başarılı dönsün
            _mockUow.Setup(x => x.CommitAsync()).ReturnsAsync(1);

            // --- ACT ---
            var result = await _sut.UpdateAsync(employee.Id, updateDto);

            // --- ASSERT ---
            result.IsSuccessful.Should().BeTrue();
            result.StatusCode.Should().Be(204); // NoContent

            // --- VERIFY ---
            // Veritabanına kaydedildi mi?
            _mockUow.Verify(x => x.CommitAsync(), Times.Once);

            // Liste yenileme sinyali gitti mi?
            _mockSignalR.Verify(x => x.RefreshEntityListAsync("Employee"), Times.Once);

            // Dashboard yenileme sinyali GİTMEMELİ (Kodda yoktu)
            _mockSignalR.Verify(x => x.RefreshDashboardAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFail_WhenIdDoesNotExist()
        {
            // --- ARRANGE ---
            int nonExistentId = 999;
            var updateDto = new EmployeeUpdateDto { FirstName = "Kimse Yok" };

            // Repository: Kayıt bulunamadı (null) dönsün
            _mockEmployeeRepo.Setup(x => x.GetByIdAsync(nonExistentId))
                .ReturnsAsync((Employee)null);

            // --- ACT ---
            var result = await _sut.UpdateAsync(nonExistentId, updateDto);

            // --- ASSERT ---
            result.IsSuccessful.Should().BeFalse();
            result.StatusCode.Should().Be(404); // Not Found

            // --- VERIFY ---
            // Kayıt yoksa ASLA commit (kaydetme) çalışmamalı
            _mockUow.Verify(x => x.CommitAsync(), Times.Never);

            // SignalR hiç çalışmamalı
            _mockSignalR.Verify(x => x.RefreshEntityListAsync(It.IsAny<string>()), Times.Never);
        }
    }
}