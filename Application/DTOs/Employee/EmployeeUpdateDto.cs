// EmployeeUpdateDto.cs
namespace Application.DTOs.Employee
{
    public class EmployeeUpdateDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Role { get; set; }
        public int? DepartmentId { get; set; }
        public DateTime? EndDate { get; set; }
    }
}