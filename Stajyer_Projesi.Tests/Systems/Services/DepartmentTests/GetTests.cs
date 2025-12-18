using Application.DTOs.Department;
using Domain.Entities;
using FluentAssertions;
using MockQueryable;
using MockQueryable.Moq; // BU EKLENMELİ (BuildMock için)
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Stajyer_Projesi.Tests.Systems.Services.DepartmentTests
{
    public class GetTests : DepartmentTestBase
    {
        [Fact]
        public async Task GetAllAsync_ShouldReturnList()
        {
            // --- ARRANGE ---
            var departments = _deptFaker.Generate(3);
            var dtos = departments.Select(d => new DepartmentListDto { Id = d.Id, Name = d.Name }).ToList();

            // HATA BURADAYDI: GetAllAsync() yok -> GetAll() var.
            // AYRICA: Service içinde ToListAsync() kullanıldığı için "BuildMock()" kullanmalıyız.

            // İçini boş bıraktık veya params dizisi beklediği için IsAny kullandık
            _mockRepo.Setup(x => x.GetAll(It.IsAny<Expression<Func<Department, object>>[]>()))
                .Returns(departments.BuildMock());

            // NOT: Eğer GetAll() parametre almıyorsa sadece x.GetAll() yaz.
            // Genelde GenericRepo'da: IQueryable<T> GetAll(bool tracking = true); şeklindedir.
            // Garanti olsun istersen: x.GetAll(It.IsAny<bool>()) yazabilirsin.

            // Mapper ayarı
            _mockMapper.Setup(x => x.Map<IEnumerable<DepartmentListDto>>(It.IsAny<List<Department>>()))
                .Returns(dtos);

            // --- ACT ---
            var result = await _sut.GetAllAsync();

            // --- ASSERT ---
            result.IsSuccessful.Should().BeTrue();
            result.Data.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnData_WhenIdExists()
        {
            // --- ARRANGE ---
            var dept = _deptFaker.Generate();
            var dto = new DepartmentListDto { Id = dept.Id, Name = dept.Name };

            _mockRepo.Setup(x => x.GetByIdAsync(dept.Id)).ReturnsAsync(dept);

            _mockMapper.Setup(x => x.Map<DepartmentListDto>(dept)).Returns(dto);

            // --- ACT ---
            var result = await _sut.GetByIdAsync(dept.Id);

            // --- ASSERT ---
            result.IsSuccessful.Should().BeTrue();
            result.Data.Name.Should().Be(dept.Name);
        }
    }
}