using API.Controllers;
using Application.Common;
using Application.DTOs.Department;
using Application.Services.Interfaces; // IDepartmentService burada
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Stajyer_Projesi.Tests.Systems.Controllers
{
    public class DepartmentControllerTests
    {
        private readonly Mock<IDepartmentService> _mockService;
        private readonly DepartmentController _controller;

        public DepartmentControllerTests()
        {
            _mockService = new Mock<IDepartmentService>();
            // Controller'ı ayağa kaldırıp mock servisi enjekte ediyoruz
            _controller = new DepartmentController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WhenDataExists()
        {
            // --- ARRANGE ---
            // Servis başarılı bir liste dönsün (Response<T> yapısı)
            var departmentList = new List<DepartmentListDto>
            {
                new DepartmentListDto { Id = 1, Name = "HR" }
            };
            var response = Response<IEnumerable<DepartmentListDto>>.Success(departmentList, 200);

            _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(response);

            // --- ACT ---
            var result = await _controller.GetAll();

            // --- ASSERT ---
            // Sonuç 200 OK olmalı
            var okResult = result as ObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);

            // Dönen veriyi kontrol et
            var returnData = okResult.Value as Response<IEnumerable<DepartmentListDto>>;
            returnData.Data.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenFound()
        {
            // --- ARRANGE ---
            int testId = 1;
            var deptDto = new DepartmentListDto { Id = testId, Name = "IT" };
            var response = Response<DepartmentListDto>.Success(deptDto, 200);

            _mockService.Setup(x => x.GetByIdAsync(testId)).ReturnsAsync(response);

            // --- ACT ---
            var result = await _controller.GetById(testId);

            // --- ASSERT ---
            var okResult = result as ObjectResult;
            okResult.StatusCode.Should().Be(200);

            var returnData = okResult.Value as Response<DepartmentListDto>;
            returnData.Data.Name.Should().Be("IT");
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenServiceReturns404()
        {
            // --- ARRANGE ---
            int testId = 99;
            // Servis "Bulunamadı" (404) dönüyor
            var response = Response<DepartmentListDto>.Fail("Kayıt bulunamadı", 404, true);

            _mockService.Setup(x => x.GetByIdAsync(testId)).ReturnsAsync(response);

            // --- ACT ---
            var result = await _controller.GetById(testId);

            // --- ASSERT ---
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Create_ShouldReturnCreated_WhenSuccessful()
        {
            // --- ARRANGE ---
            var createDto = new DepartmentCreateDto { Name = "Yeni Departman" };
            // Servis "Oluşturuldu" (201) dönüyor
            var response = Response<int>.Success(1, 201);

            _mockService.Setup(x => x.CreateAsync(createDto)).ReturnsAsync(response);

            // --- ACT ---
            var result = await _controller.Create(createDto);

            // --- ASSERT ---
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenNameExists()
        {
            // --- ARRANGE ---
            var createDto = new DepartmentCreateDto { Name = "Mevcut Departman" };
            // Servis "Zaten var" (400) dönüyor
            var response = Response<int>.Fail("Bu isimde departman var", 400, true);

            _mockService.Setup(x => x.CreateAsync(createDto)).ReturnsAsync(response);

            // --- ACT ---
            var result = await _controller.Create(createDto);

            // --- ASSERT ---
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenSuccessful()
        {
            // --- ARRANGE ---
            int id = 1;
            var updateDto = new DepartmentUpdateDto { Name = "Guncel Isim" };
            // Servis "NoContent" (204) dönüyor
            var response = Response<NoContent>.Success(204);

            _mockService.Setup(x => x.UpdateAsync(id, updateDto)).ReturnsAsync(response);

            // --- ACT ---
            var result = await _controller.Update(id, updateDto);

            // --- ASSERT ---
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(204);
        }

        [Fact]
        public async Task Delete_ShouldReturnBadRequest_WhenEmployeesExist()
        {
            // --- ARRANGE ---
            int id = 1;
            // Servis "İçinde çalışan var, silemezsin" (400) dönüyor
            var response = Response<NoContent>.Fail("Çalışan var", 400, true);

            _mockService.Setup(x => x.DeleteAsync(id)).ReturnsAsync(response);

            // --- ACT ---
            var result = await _controller.Delete(id);

            // --- ASSERT ---
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(400);
        }
    }
}