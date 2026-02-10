using Application.DTOs.Assignment;
using Application.DTOs.Department;
using Application.DTOs.Employee;
using Application.DTOs.Intern;
using Application.DTOs.InventoryItem;
using Application.DTOs.Maintenance;
using Application.DTOs.University;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ======================================================
            // UNIVERSITY MAPPING
            // ======================================================
            CreateMap<UniversityCreateDto, University>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Students, opt => opt.Ignore())
                .ForMember(dest => dest.City, opt => opt.Ignore());

            CreateMap<UniversityUpdateDto, University>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Students, opt => opt.Ignore())
                .ForMember(dest => dest.City, opt => opt.Ignore());

            CreateMap<University, UniversityListDto>()
                .ForMember(dest => dest.Country, opt => opt.Ignore());


            // ======================================================
            // DEPARTMENT MAPPING
            // ======================================================
            CreateMap<DepartmentCreateDto, Department>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Employees, opt => opt.Ignore());

            CreateMap<DepartmentUpdateDto, Department>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Employees, opt => opt.Ignore());

            CreateMap<Department, DepartmentListDto>().ReverseMap();


            // ======================================================
            // INTERN MAPPING
            // ======================================================
            CreateMap<InternCreateDto, Intern>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.University, opt => opt.Ignore())
                .ForMember(dest => dest.Assignments, opt => opt.Ignore());

            CreateMap<InternUpdateDto, Intern>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.University, opt => opt.Ignore())
                .ForMember(dest => dest.Assignments, opt => opt.Ignore());

            CreateMap<Intern, InternListDto>()
                .ForMember(dest => dest.UniversityName, opt => opt.MapFrom(src => src.University.Name));


            // ======================================================
            // EMPLOYEE MAPPING
            // ======================================================
            CreateMap<EmployeeCreateDto, Employee>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.AppUser, opt => opt.Ignore())
                .ForMember(dest => dest.AppUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Assignments, opt => opt.Ignore());

            CreateMap<EmployeeUpdateDto, Employee>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.AppUser, opt => opt.Ignore())
                .ForMember(dest => dest.AppUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Assignments, opt => opt.Ignore())
                .ForMember(dest => dest.EndDate, opt => opt.Ignore());

            CreateMap<Employee, EmployeeListDto>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null));


            // ======================================================
            // INVENTORY ITEM MAPPING
            // ======================================================
            CreateMap<InventoryItemCreateDto, InventoryItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Assignments, opt => opt.Ignore())
                .ForMember(dest => dest.Maintenances, opt => opt.Ignore());

            CreateMap<InventoryItemUpdateDto, InventoryItem>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Assignments, opt => opt.Ignore())
                .ForMember(dest => dest.Maintenances, opt => opt.Ignore());

            CreateMap<InventoryItem, InventoryItemListDto>();


            // ======================================================
            // ASSIGNMENT MAPPING
            // ======================================================
            CreateMap<AssignmentCreateDto, Assignment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.InventoryItem, opt => opt.Ignore())
                .ForMember(dest => dest.Intern, opt => opt.Ignore())
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForMember(dest => dest.ActualReturnAt, opt => opt.Ignore());

            CreateMap<AssignmentUpdateDto, Assignment>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.InventoryItem, opt => opt.Ignore())
                .ForMember(dest => dest.Intern, opt => opt.Ignore())
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForMember(dest => dest.InventoryItemId, opt => opt.Ignore())
                .ForMember(dest => dest.InternId, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedAt, opt => opt.Ignore());

            CreateMap<Assignment, AssignmentListDto>()
                .ForMember(dest => dest.ItemCode, opt => opt.MapFrom(src => src.InventoryItem.ItemCode))
                .ForMember(dest => dest.AssigneeName, opt => opt.MapFrom(src =>
                    src.Intern != null ? src.Intern.FullName :
                    src.Employee != null ? src.Employee.FullName : null));


            // ======================================================
            // MAINTENANCE MAPPING
            // ======================================================
            CreateMap<MaintenanceCreateDto, Maintenance>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.InventoryItem, opt => opt.Ignore())
                .ForMember(dest => dest.RepairedAt, opt => opt.Ignore());

            CreateMap<MaintenanceUpdateDto, Maintenance>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // <--- BU SATIR EKLENDİ
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.InventoryItem, opt => opt.Ignore())
                .ForMember(dest => dest.InventoryItemId, opt => opt.Ignore())
                .ForMember(dest => dest.ReportedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.PerformedBy, opt => opt.Ignore());

            CreateMap<Maintenance, MaintenanceListDto>()
                .ForMember(dest => dest.InventoryItemCode, opt => opt.MapFrom(src => src.InventoryItem != null ? src.InventoryItem.ItemCode : null));
        }
    }
}