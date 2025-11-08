// AssignmentListDto.cs
namespace Application.DTOs.Assignment
{
    public class AssignmentListDto
    {
        public int Id { get; set; }
        public string ItemCode { get; set; } = null!; // InventoryItem code
        public string? AssigneeName { get; set; } // Student veya Employee adı
        public DateTime AssignedAt { get; set; }
        public DateTime? ExpectedReturnAt { get; set; }
        public DateTime? ActualReturnAt { get; set; }
        public string? Notes { get; set; }
    }
}