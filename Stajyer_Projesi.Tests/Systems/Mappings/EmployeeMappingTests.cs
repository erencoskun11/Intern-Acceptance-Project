using Application.DTOs.Employee;
using Application.Mappings;
using AutoMapper;
using Domain.Entities;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stajyer_Projesi.Tests.Systems.Mappings
{
    public class EmployeeMappingTests
    {
        private readonly IMapper _mapper;
        public EmployeeMappingTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = config.CreateMapper();
        }

        [Fact]
        public void Configuration_Should_Be_Valid()
        {
            var config = _mapper.ConfigurationProvider;
            config.AssertConfigurationIsValid();
        }

        [Fact]
        public void Should_Map_Employee_To_EmployeeListDto_Correctly()
        {
            var employee = new Employee { Id = 1, FirstName = "Test", LastName="User"};

            var dto = _mapper.Map<EmployeeListDto>(employee);

            dto.Should().NotBeNull();
            dto.FirstName.Should().Be("Test");
            dto.LastName.Should().Be("User");

        }











    }
}
