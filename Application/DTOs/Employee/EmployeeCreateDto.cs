// EmployeeCreateDto.cs
namespace Application.DTOs.Employee
{
    public class EmployeeCreateDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Role { get; set; } = "User";
        public int? DepartmentId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Password { get; set; } = null!;
    }
}