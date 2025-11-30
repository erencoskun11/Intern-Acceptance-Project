namespace Application.DTOs.Employee
{
    public class EmployeeListDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; } = null!;
    }
}