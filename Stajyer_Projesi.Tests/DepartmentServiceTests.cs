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

        [Fact]
        public async Task GetAllAsync_Should_Fetch_From_Db_And_Set_Cache_When_Cache_Is_Empty()
        {
            IEnumerable<DepartmentListDto> outList = null;
            _cacheMock.Setup(x => x.TryGet(CacheKeys.DepartmentList, out outList))
                      .Returns(false);

            var dbList = new List<Department> { new Department { Id = 1, Name = "HR" } };
            var mockQuery = dbList.AsQueryable().BuildMock();

            _deptRepoMock.Setup(x => x.GetAll()).Returns(mockQuery);

            _mapperMock.Setup(m => m.Map<IEnumerable<DepartmentListDto>>(It.IsAny<List<Department>>()))
                       .Returns(new List<DepartmentListDto> { new DepartmentListDto { Name = "HR" } });

            var result = await _service.GetAllAsync();

            result.IsSuccessful.Should().BeTrue();
            result.Data.First().Name.Should().Be("HR");

            _deptRepoMock.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_Clear_Cache_After_Success()
        {
            var dto = new DepartmentCreateDto { Name = "New Dept" };
            var entity = new Department { Id = 10, Name = "New Dept" };

            _mapperMock.Setup(m => m.Map<Department>(dto)).Returns(entity);

            var result = await _service.CreateAsync(dto);

            result.IsSuccessful.Should().BeTrue();

            _cacheMock.Verify(x => x.Remove(CacheKeys.DepartmentList), Times.Once);
        }
    }
}