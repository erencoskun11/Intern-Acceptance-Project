namespace Application.DTOs.Employee
{
    public class EmployeeUpdateDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Role { get; set; } = null!;
        public int? DepartmentId { get; set; }
        public DateTime StartDate { get; set; }
    }
}