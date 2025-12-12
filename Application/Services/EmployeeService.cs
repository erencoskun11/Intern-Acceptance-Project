using Application.Common;
using Application.DTOs.Employee;
using Application.Interfaces;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class EmployeeService : GenericService<Employee, EmployeeListDto, EmployeeCreateDto, EmployeeUpdateDto>, IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository repository, IMapper mapper, IUnitOfWork uow)
            : base(repository, mapper, uow)
        {
            _employeeRepository = repository;
        }

        public override async Task<Response<int>> CreateAsync(EmployeeCreateDto dto)
        {
            var existing = await _employeeRepository.GetByEmailAsync(dto.Email);
            if (existing != null)
                return Response<int>.Fail("Bu e-posta adresi zaten kullanımda.", 400, true);

            return await base.CreateAsync(dto);
        }

        public async Task<IEnumerable<EmployeeListDto>> GetByDepartmentIdAsync(int departmentId)
        {
            var query = _employeeRepository.GetByDepartmentId(departmentId);
            var employees = await query.ToListAsync();
            return _mapper.Map<IEnumerable<EmployeeListDto>>(employees);
        }

        public async Task<EmployeeListDto?> GetByEmailAsync(string email)
        {
            var employee = await _employeeRepository.GetByEmailAsync(email);
            return employee == null ? null : _mapper.Map<EmployeeListDto>(employee);
        }
    }
}