// EmployeeListDto.cs
namespace Application.DTOs.Employee
{
    public class EmployeeListDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Role { get; set; } = null!;
        public int? DepartmentId { get; set; }

        public string? DepartmentName {  get; set; }=null!;   
    }
}