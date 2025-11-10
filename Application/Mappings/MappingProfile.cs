using AutoMapper;
using Domain.Entities;
using Application.DTOs.University;
using Application.DTOs.Department;
using Application.DTOs.Student;
using Application.DTOs.InternshipApplication;
using Application.DTOs.Employee;
using Application.DTOs.InventoryItem;
using Application.DTOs.Assignment;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ===== University =====
            CreateMap<UniversityCreateDto, University>();
            CreateMap<UniversityUpdateDto, University>();
            CreateMap<University, UniversityListDto>();

            // ===== Department =====
            CreateMap<DepartmentCreateDto, Department>();
            CreateMap<DepartmentUpdateDto, Department>();
            CreateMap<Department, DepartmentListDto>();

            // ===== Student =====
            CreateMap<StudentCreateDto, Student>();
            CreateMap<StudentUpdateDto, Student>();
            CreateMap<Student, StudentListDto>()
                .ForMember(dest => dest.UniversityName, opt => opt.MapFrom(src => src.University.Name));

            // ===== Employee =====
            CreateMap<EmployeeCreateDto, Employee>();
            CreateMap<EmployeeUpdateDto, Employee>();
            CreateMap<Employee, EmployeeListDto>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null));

          

            // ===== InternshipApplication =====
            CreateMap<InternshipApplicationCreateDto, InternshipApplication>();
            CreateMap<InternshipApplicationUpdateDto, InternshipApplication>();
            CreateMap<InternshipApplication, InternshipApplicationListDto>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.FullName))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.FullName : null));

            // ===== InventoryItem =====
            CreateMap<InventoryItemCreateDto, InventoryItem>();
            CreateMap<InventoryItemUpdateDto, InventoryItem>();
            CreateMap<InventoryItem, InventoryItemListDto>();

            // ===== Assignment =====
            CreateMap<AssignmentCreateDto, Assignment>();
            CreateMap<AssignmentUpdateDto, Assignment>();
            CreateMap<Assignment, AssignmentListDto>()
                .ForMember(dest => dest.ItemCode, opt => opt.MapFrom(src => src.InventoryItem.ItemCode))
                .ForMember(dest => dest.AssigneeName, opt => opt.MapFrom(src =>
                    src.Student != null ? src.Student.FullName :
                    src.Employee != null ? src.Employee.FullName : null));
        }
    }
}
