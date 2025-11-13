
// MaintenanceCreateDto.cs
namespace Application.DTOs.Maintenance
{
    public class MaintenanceCreateDto
    {
        public int InventoryItemId { get; set; }
        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
        public string? Description { get; set; }
        public string? PerformedBy { get; set; }
        public decimal? Cost { get; set; }
    }
}