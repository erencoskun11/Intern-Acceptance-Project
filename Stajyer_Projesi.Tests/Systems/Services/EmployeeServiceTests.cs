using Application.DTOs.Employee;
using Application.Interfaces;
using Application.Services;
using Application.Services.Interfaces;
using Application.Common;
using AutoMapper;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Stajyer_Projesi.Tests.Systems.Services
{
    public class EmployeeServiceTests
    {
        // 1. DEĞİŞİKLİK: Generic yerine IEmployeeRepository kullanıyoruz.
        private readonly Mock<IEmployeeRepository> _mockEmployeeRepo;

        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<ISignalRService> _mockSignalR;
        // Cache servisi kodunuzda olmadığı için buradan kaldırdık.

        private readonly EmployeeService _sut; // System Under Test

        public EmployeeServiceTests()
        {
            _mockEmployeeRepo = new Mock<IEmployeeRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockUow = new Mock<IUnitOfWork>();
            _mockSignalR = new Mock<ISignalRService>();

            // 2. DEĞİŞİKLİK: Constructor parametrelerini kodunuzla birebir aynı yaptık.
            // Sıralama: (Repo, Mapper, UoW, SignalR)
            _sut = new EmployeeService(
                _mockEmployeeRepo.Object,
                _mockMapper.Object,
                _mockUow.Object,
                _mockSignalR.Object
            );
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnFail_WhenEmailAlreadyExist()
        {
            // --- ARRANGE ---
            var employeeDto = new EmployeeCreateDto
            {
                Email = "varolan@sirket.com",
                Phone = "5551112233",
                FirstName = "Test",
                LastName = "User"
            };

            // Senaryo: Email sorulduğunda veritabanından bir kayıt dönsün.
            _mockEmployeeRepo.Setup(x => x.GetByEmailAsync(employeeDto.Email))
                .ReturnsAsync(new Employee { Id = 1, Email = "varolan@sirket.com" });

            // --- ACT ---
            var result = await _sut.CreateAsync(employeeDto);

            // --- ASSERT ---
            result.IsSuccessful.Should().BeFalse();
            result.StatusCode.Should().Be(400);

            // Hata mesajı kontrolü
            result.Error.Should().NotBeNull();
            if (result.Error.Errors != null)
            {
                result.Error.Errors.First().Should().Contain("Aynı gmaille stajyer veya çalışan kayıt edemezsiniz");
            }

            // Veritabanına asla ekleme yapılmamalı
            _mockEmployeeRepo.Verify(x => x.AddAsync(It.IsAny<Employee>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnFail_WhenPhoneAlreadyExist()
        {
            // --- ARRANGE ---
            var employeeDto = new EmployeeCreateDto
            {
                Email = "yeni@sirket.com", // Email temiz
                Phone = "5559998877",      // Bu telefon zaten var olsun
                FirstName = "Test",
                LastName = "User"
            };

            // 1. Email temiz (null) dönsün
            _mockEmployeeRepo.Setup(x => x.GetByEmailAsync(employeeDto.Email))
                .ReturnsAsync((Employee)null);

            // 2. Telefon kontrolü (AnyAsync) TRUE dönsün (Yani kayıt var)
            _mockEmployeeRepo.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Employee, bool>>>()))
                .ReturnsAsync(true);

            // --- ACT ---
            var result = await _sut.CreateAsync(employeeDto);

            // --- ASSERT ---
            result.IsSuccessful.Should().BeFalse();
            result.StatusCode.Should().Be(400);

            // Telefon hatası mesajını kontrol et
            result.Error.Should().NotBeNull();
            if (result.Error.Errors != null)
            {
                result.Error.Errors.First().Should().Contain("Aynı telefon numarası ile stajyer veya çalışan kabul edemezsiniz");
            }

            _mockEmployeeRepo.Verify(x => x.AddAsync(It.IsAny<Employee>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnSuccess_WhenEmailAndPhoneAreUnique()
        {
            // --- ARRANGE ---
            var employeeDto = new EmployeeCreateDto
            {
                Email = "yeni@sirket.com",
                Phone = "5550000000",
                FirstName = "Yeni",
                LastName = "Personel"
            };

            // 1. Email yok (null)
            _mockEmployeeRepo.Setup(x => x.GetByEmailAsync(employeeDto.Email))
                .ReturnsAsync((Employee)null);

            // 2. Telefon yok (false)
            _mockEmployeeRepo.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Employee, bool>>>()))
                .ReturnsAsync(false);

            // 3. Mapper ayarı
            _mockMapper.Setup(x => x.Map<Employee>(employeeDto))
                .Returns(new Employee { Id = 1, Email = "yeni@sirket.com" });

            // --- ACT ---
            var result = await _sut.CreateAsync(employeeDto);

            // --- ASSERT ---
            result.IsSuccessful.Should().BeTrue();

            // AddAsync ve CommitAsync birer kez çalışmalı
            _mockEmployeeRepo.Verify(x => x.AddAsync(It.IsAny<Employee>()), Times.Once);
            _mockUow.Verify(x => x.CommitAsync(), Times.Once);

            // SignalR bildirimleri gitmeli
            _mockSignalR.Verify(x => x.SendNotificationAsync(It.IsAny<string>(), "success"), Times.Once);
        }


        [Fact]
        public async Task GetByIdAsync_ShouldReturnData_WhenIdExist()
        {
            //Arrange
            int testId = 1;
            var employee = new Employee { Id = testId, FirstName = "Ali", LastName = "Veli" };


            _mockEmployeeRepo.Setup(x=>x.GetByIdAsync(testId))
                .ReturnsAsync(employee);



            _mockMapper.Setup(x=>x.Map<EmployeeListDto>(employee))
                .Returns(new EmployeeListDto { Id = testId, FirstName = "Ali", LastName = "Veli"});

            //Act
            var result = await _sut.GetByIdAsync(testId);

            //Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(testId);
            result.IsSuccessful.Should().BeTrue();
        }
        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenIdDoesNotExist()
        {
            int nonExistentId = 999;

            _mockEmployeeRepo.Setup(x => x.GetByIdAsync(nonExistentId))
                .ReturnsAsync((Employee)null);

            //Act

            var result = await _sut.GetByIdAsync(nonExistentId);

            //ASSERT 
            result.Should().NotBeNull();

        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenIdExists()
        {
            //Arrange
            int testId = 5;

            var employeeToDelete = new Employee { Id = testId, FirstName = "Silinecek", LastName = "Kisi" };

            _mockEmployeeRepo.Setup(x => x.GetByIdAsync(testId))
                .ReturnsAsync(employeeToDelete);


            //Act
            var result = await _sut.DeleteAsync(testId);

            //Assert
            result.IsSuccessful.Should().BeTrue();
            result.StatusCode.Should().Be(204);

            //Remove metodu cagrıldımı 
            _mockEmployeeRepo.Verify(x => x.Remove(employeeToDelete), Times.Once);

            _mockUow.Verify(x => x.CommitAsync(), Times.Once);

            _mockSignalR.Verify(x => x.RefreshDashboardAsync(), Times.Once);

        }

       


    }
}