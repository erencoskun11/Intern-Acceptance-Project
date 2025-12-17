using Application.DTOs.Employee;
using Application.Services;
using Application.Interfaces;
using Application.Services.Interfaces;
using AutoMapper;
using Bogus;
using Domain.Entities;
using Moq;

namespace Stajyer_Projesi.Tests.Systems.Services.EmployeeTests
{
    public class EmployeeTestBase
    {
        // "protected" yapıyoruz ki miras alan sınıflar erişebilsin
        protected readonly Mock<IEmployeeRepository> _mockEmployeeRepo;
        protected readonly Mock<IMapper> _mockMapper;
        protected readonly Mock<IUnitOfWork> _mockUow;
        protected readonly Mock<ISignalRService> _mockSignalR;
        protected readonly EmployeeService _sut; // System Under Test

        protected readonly Faker<Employee> _employeeFaker;
        protected readonly Faker<EmployeeCreateDto> _createDtoFaker;

        public EmployeeTestBase()
        {
            _mockEmployeeRepo = new Mock<IEmployeeRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockUow = new Mock<IUnitOfWork>();
            _mockSignalR = new Mock<ISignalRService>();

            _sut = new EmployeeService(
                _mockEmployeeRepo.Object,
                _mockMapper.Object,
                _mockUow.Object,
                _mockSignalR.Object
            );

            // Faker Kuralları
            _employeeFaker = new Faker<Employee>()
                .RuleFor(e => e.Id, f => f.Random.Int(1, 100))
                .RuleFor(e => e.FirstName, f => f.Name.FirstName())
                .RuleFor(e => e.LastName, f => f.Name.LastName())
                .RuleFor(e => e.Email, (f, e) => f.Internet.Email(e.FirstName, e.LastName))
                .RuleFor(e => e.Phone, f => f.Phone.PhoneNumber("5#########"));

            _createDtoFaker = new Faker<EmployeeCreateDto>()
                .RuleFor(d => d.FirstName, f => f.Name.FirstName())
                .RuleFor(d => d.LastName, f => f.Name.LastName())
                .RuleFor(d => d.Email, (f, d) => f.Internet.Email(d.FirstName, d.LastName))
                .RuleFor(d => d.Phone, f => f.Phone.PhoneNumber("5#########"))
                .RuleFor(d => d.DepartmentId, f => f.Random.Int(1, 5))
                .RuleFor(d => d.Role, "User");
        }
    }
}