using Application.DTOs.Department;
using Application.Interfaces;
using Application.Services;
using Application.Services.Interfaces;
using AutoMapper;
using Bogus;
using Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stajyer_Projesi.Tests.Systems.Services.DepartmentTests
{
    public class DepartmentTestBase
    {
        protected readonly Mock<IGenericRepository<Department>> _mockRepo;
        protected readonly Mock<IMapper> _mockMapper;
        protected readonly Mock<IUnitOfWork> _mockUow;
        protected readonly DepartmentService _sut;
        protected readonly Mock<ICacheService> _mockCache;
        protected readonly Faker<Department> _deptFaker;
        protected readonly Faker<DepartmentCreateDto> _createDtoFaker;

        public DepartmentTestBase()
        {
            _mockRepo = new Mock<IGenericRepository<Department>>();
            _mockMapper = new Mock<IMapper>();
            _mockUow = new Mock<IUnitOfWork>();
            _mockCache = new Mock<ICacheService>();

            _sut = new DepartmentService(
                _mockRepo.Object,
                _mockMapper.Object,
                _mockUow.Object,
                _mockCache.Object
            );
            // Bogus Kuralları
            _deptFaker = new Faker<Department>()
                .RuleFor(d => d.Id, f => f.Random.Int(1, 10))
                .RuleFor(d => d.Name, f => f.Commerce.Department());

            _createDtoFaker = new Faker<DepartmentCreateDto>()
                .RuleFor(d => d.Name, f => f.Commerce.Department());
        }





    }
}
