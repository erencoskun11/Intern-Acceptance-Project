using Application.DTOs.Department;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;
using System.Linq.Expressions;

namespace Stajyer_Projesi.Tests.Systems.Services.DepartmentTests
{
    public class CreateTests : DepartmentTestBase
    {
        [Fact]
        public async Task CreateAsync_ShouldReturnSuccess_WhenNameIsUnique()
        {
            // --- ARRANGE ---
            var createDto = _createDtoFaker.Generate();

            // 1. İsim kontrolü FALSE dönsün (Yani kayıt yok, devam et)
            _mockRepo.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Department, bool>>>()))
                .ReturnsAsync(false);

            // 2. Mapper Setup (DTO -> Entity)
            // It.IsAny<DepartmentCreateDto>() kullanarak garantiye alıyoruz
            _mockMapper.Setup(x => x.Map<Department>(It.IsAny<DepartmentCreateDto>()))
                .Returns(new Department { Name = createDto.Name });

            // 3. (ÖNEMLİ) AddAsync çağrısı hata vermesin (Task dönsün)
            _mockRepo.Setup(x => x.AddAsync(It.IsAny<Department>()))
                .Returns(Task.CompletedTask);

            // 4. (ÖNEMLİ) CommitAsync 1 dönsün (GenericService burada patlıyor olabilir)
            _mockUow.Setup(x => x.CommitAsync())
                .ReturnsAsync(1);

            // --- ACT ---
            var result = await _sut.CreateAsync(createDto);

            // --- ASSERT ---
            result.IsSuccessful.Should().BeTrue();
            result.StatusCode.Should().Be(201);

            _mockRepo.Verify(x => x.AddAsync(It.IsAny<Department>()), Times.Once);
            _mockUow.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnFail_WhenNameExists()
        {
            // --- ARRANGE ---
            var createDto = _createDtoFaker.Generate();

            // 1. İsim kontrolü TRUE dönsün (Zaten var, HATA VERMELİ)
            _mockRepo.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Department, bool>>>()))
                .ReturnsAsync(true);

            // NOT: Burada Mapper veya UoW setup'ına gerek yok.
            // Çünkü kod if(nameExists) bloğuna girip return olacak.
            // Aşağıdaki base.CreateAsync'e hiç inmeyecek.

            // --- ACT ---
            var result = await _sut.CreateAsync(createDto);

            // --- ASSERT ---
            result.IsSuccessful.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Error.Errors.First().Should().Contain("mevcut");

            // Ekleme metoduna hiç gidilmemeli
            _mockRepo.Verify(x => x.AddAsync(It.IsAny<Department>()), Times.Never);
        }
    }
}