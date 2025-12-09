using Application.Constants; 
using Application.DTOs.Department; 
using Application.Interfaces;
using Application.Services;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentAssertions;
using MockQueryable.Moq; 
using Moq;

namespace Stajyer_Projesi.Tests
{
    public class DepartmentServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICacheService> _cacheMock;
        private readonly Mock<IGenericRepository<Department>> _deptRepoMock;

        private readonly DepartmentService _service;

        public DepartmentServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _cacheMock = new Mock<ICacheService>();
            _deptRepoMock = new Mock<IGenericRepository<Department>>();

            _service = new DepartmentService(
                _deptRepoMock.Object,
                _mapperMock.Object,
                _uowMock.Object,
                _cacheMock.Object
            );
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_From_Cache_When_Cache_Is_Populated()
        {
            var cachedList = new List<DepartmentListDto>
            {
                new DepartmentListDto { Id = 1, Name = "IT (Cached)" }
            };

            IEnumerable<DepartmentListDto> outValue = cachedList;

            _cacheMock.Setup(x => x.TryGet(CacheKeys.DepartmentList, out outValue))
                      .Returns(true);

            var result = await _service.GetAllAsync();

            result.IsSuccessful.Should().BeTrue();
            result.Data.Should().HaveCount(1);
            result.Data.First().Name.Should().Be("IT (Cached)");

            _deptRepoMock.Verify(x => x.GetAll(), Times.Never);
        }

       
    }
}