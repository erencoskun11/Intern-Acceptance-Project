using Application.DTOs.Department;
using Application.Interfaces;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class DepartmentService : GenericService<Department, DepartmentListDto, DepartmentCreateDto, DepartmentUpdateDto>, IDepartmentService
    {
        public DepartmentService(IGenericRepository<Department> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}
