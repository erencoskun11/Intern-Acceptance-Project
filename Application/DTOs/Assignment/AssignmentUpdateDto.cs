// AssignmentUpdateDto.cs
namespace Application.DTOs.Assignment
{
    public class AssignmentUpdateDto
    {
        public int Id { get; set; }
        public DateTime? ActualReturnAt { get; set; }
        public string? Notes { get; set; }
    }
}