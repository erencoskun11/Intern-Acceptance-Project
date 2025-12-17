using API.Controllers;
using Application.Common;
using Application.DTOs.Employee;
using Application.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace Stajyer_Projesi.Tests.Systems.Controllers
{
    public class EmployeeControllerTests
    {
        private readonly Mock<IEmployeeService> _mockService;
        private readonly EmployeeController _controller;

        public EmployeeControllerTests()
        {
            _mockService = new Mock<IEmployeeService>();
            _controller = new EmployeeController(_mockService.Object);
        }

        [Fact]
        public async Task GetById_Should_Return_Ok_When_Found()
        {
            //Arrange
            int id = 1;
            var response = Response<EmployeeListDto>.Success(new EmployeeListDto(), 200);

            _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(response);
            //Act
            var result = await _controller.GetById(id);

            //Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Create_Should_Return_BadRequest_When_Service_Fails()
        {
            //Arrange
            var dto=new EmployeeCreateDto();
            var response=Response<int>.Fail("Hata",400,true);
            _mockService.Setup(s => s.CreateAsync(dto)).ReturnsAsync(response);


            //Act
            var result = await _controller.Create(dto);

            //Assert
            var badRequest = result as ObjectResult;
            badRequest.StatusCode.Should().Be(400);
        }



    }
}
