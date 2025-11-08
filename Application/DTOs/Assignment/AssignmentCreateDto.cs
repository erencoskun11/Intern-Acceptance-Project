// AssignmentCreateDto.cs
namespace Application.DTOs.Assignment
{
    public class AssignmentCreateDto
    {
        public int InventoryItemId { get; set; }
        public int? EmployeeId { get; set; }
        public int? StudentId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpectedReturnAt { get; set; }
        public string? Notes { get; set; }
    }
}